using System;
using Microsoft.SPOT.Hardware;

namespace Pn532Test
{
    public class I2CBus : IDisposable
    {
        private static I2CBus _instance;
        private static readonly object LockObject = new object();

        public static I2CBus GetInstance()
        {
            lock (LockObject)
            {
                return _instance ?? (_instance = new I2CBus());
            }
        }


        private readonly I2CDevice _slaveDevice;

        private I2CBus()
        {
            _slaveDevice = new I2CDevice(new I2CDevice.Configuration(0, 0));
        }

        public void Dispose()
        {
            _slaveDevice.Dispose();
        }

        /// <summary>
        /// Generic write operation to I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="writeBuffer">The array of bytes that will be sent to the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void Write(I2CDevice.Configuration config, byte[] writeBuffer, int transactionTimeout)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (writeBuffer == null) throw new ArgumentNullException("writeBuffer");

            // Set i2c device configuration.
            _slaveDevice.Config = config;

            // create an i2c write transaction to be sent to the device.
            var writeXAction = new I2CDevice.I2CTransaction[] { I2CDevice.CreateWriteTransaction(writeBuffer) };

            lock (_slaveDevice)
            {
                // the i2c data is sent here to the device.
                var transferred = _slaveDevice.Execute(writeXAction, transactionTimeout);

                // make sure the data was sent.
                if (transferred != writeBuffer.Length)
                    throw new Exception("Could not write to device.");
            }
        }

        /// <summary>
        /// Generic read operation from I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="readBuffer">The array of bytes that will contain the data read from the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void Read(I2CDevice.Configuration config, byte[] readBuffer, int transactionTimeout)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (readBuffer == null) throw new ArgumentNullException("readBuffer");

            // Set i2c device configuration.
            _slaveDevice.Config = config;

            // create an i2c read transaction to be sent to the device.
            var readXAction = new I2CDevice.I2CTransaction[] { I2CDevice.CreateReadTransaction(readBuffer) };

            lock (_slaveDevice)
            {
                // the i2c data is received here from the device.
                var transferred = _slaveDevice.Execute(readXAction, transactionTimeout);

                // make sure the data was received.
                if (transferred != readBuffer.Length)
                    throw new Exception("Could not read from device.");
            }
        }

        /// <summary>
        /// Read array of bytes at specific register from the I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="register">The register to read bytes from.</param>
        /// <param name="readBuffer">The array of bytes that will contain the data read from the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void ReadRegister(I2CDevice.Configuration config, byte register, byte[] readBuffer, int transactionTimeout)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (readBuffer == null) throw new ArgumentNullException("readBuffer");

            byte[] registerBuffer = { register };
            Write(config, registerBuffer, transactionTimeout);
            Read(config, readBuffer, transactionTimeout);
        }

        /// <summary>
        /// Write array of bytes value to a specific register on the I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="register">The register to send bytes to.</param>
        /// <param name="writeBuffer">The array of bytes that will be sent to the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void WriteRegister(I2CDevice.Configuration config, byte register, byte[] writeBuffer, int transactionTimeout)
        {
            if (config == null) throw new ArgumentNullException("config");
            if (writeBuffer == null) throw new ArgumentNullException("writeBuffer");
            
            byte[] registerBuffer = { register };
            Write(config, registerBuffer, transactionTimeout);
            Write(config, writeBuffer, transactionTimeout);
        }

        /// <summary>
        /// Write a byte value to a specific register on the I2C slave device.
        /// </summary>
        /// <param name="config">I2C slave device configuration.</param>
        /// <param name="register">The register to send bytes to.</param>
        /// <param name="value">The byte that will be sent to the device.</param>
        /// <param name="transactionTimeout">The amount of time the system will wait before resuming execution of the transaction.</param>
        public void WriteRegister(I2CDevice.Configuration config, byte register, byte value, int transactionTimeout)
        {
            if (config == null) throw new ArgumentNullException("config");

            byte[] writeBuffer = { register, value };
            Write(config, writeBuffer, transactionTimeout);
        }

    }
}
