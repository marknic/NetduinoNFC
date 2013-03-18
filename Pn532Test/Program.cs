using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;

namespace Pn532Test
{
    public class Program
    {
        const int I2CWriteAddress = 0x48;
        const int I2CReadAddress = 0x48;    // Tried 0x48 and 0x49 - documentation says PN532 is half-duplex and (0x48 is write/0x49 is read)
        const int I2CTimeout = 1000;        // Tried 5000 - didn't help
        const int I2CClockRateKhz = 400;    // Tried 200 and 400
        
        const byte Pn532Packbuffsiz = 64;
        static readonly byte[] Pn532Packetbuffer = new byte[Pn532Packbuffsiz];
        
        private static readonly OutputPort Reset = new OutputPort(Pins.GPIO_PIN_D3, false);
        private static readonly InterruptPort Irq = new InterruptPort(Pins.GPIO_PIN_D2, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeLow);

        private readonly static I2CDevice.Configuration ReadConfiguration = new I2CDevice.Configuration(I2CReadAddress, I2CClockRateKhz);
        private readonly static I2CDevice.Configuration WriteConfiguration = new I2CDevice.Configuration(I2CWriteAddress, I2CClockRateKhz);

        public static void Main()
        { 
            BoardReset();
            
            // Set up interrupt handler
            Irq.OnInterrupt += IrqOnInterrupt;

            //_nfcDevice = new I2CDevice(WriteConfiguration);

            // Try to read something
            //ReadI2CStatus();

            // Try to write ACK or NACK
            //WriteAckNack(false);

            // Try basic get firmware version
            Pn532Packetbuffer[0] = Constants.Pn532CommandGetfirmwareversion;

            var result = SendCommandCheckAck(Pn532Packetbuffer, 1, I2CTimeout);

            if (result == false) Debug.Print("SendCommandCheckAck Timed Out");
        }

        private static void IrqOnInterrupt(uint data1, uint data2, DateTime time)
        {
            Debug.Print("Interupt");
        }

        /// <summary>
        /// Reset the board.  
        /// </summary>
        static void BoardReset()
        {
            Reset.Write(true);
            Reset.Write(false);
            Thread.Sleep(400); 
            Reset.Write(true);
        }

        private static bool SendCommandCheckAck(byte[] buffer, byte bufferLength, int timeout)
        {
            //var timer = 0;

            var now = DateTime.Now;

            // write the command  
            WriteCommand(buffer, bufferLength);

            // Wait for chip to say its ready!
            while ((ReadI2CStatus() & Constants.Pn532I2CReady) != Constants.Pn532I2CReady)
            {
                if (now.AddMilliseconds(timeout) < DateTime.Now)
                {
                    return false;
                } 
                
                Thread.Sleep(20);
            }

            return true;
        }

        private static byte ReadI2CStatus()
        {
            var readBuffer = new byte[] {0x00};
            
            try
            {
                I2CBus.GetInstance().Read(ReadConfiguration, readBuffer, I2CTimeout);

                // Ready sent back?
                if ((readBuffer[0] & Constants.Pn532I2CReadbit) == Constants.Pn532I2CReady)
                {
                    Debug.Print("ready!!!!!!!");
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return readBuffer[0];
        }

        private static void WriteAckNack(bool doAck)
        {
            var frame = doAck ? new byte[] {0x00, 0x00, 0xff, 0x00, 0xff, 0x00} : new byte[] { 0x00, 0x00, 0xff, 0xff, 0x00, 0x00 };

            // print buffer to output
            frame.PrintBuffer();

            try
            {
                I2CBus.GetInstance().Write(WriteConfiguration, frame, I2CTimeout);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }

 
        /// <summary>
        /// | Preamble | Startcode (2 bytes) 0x00 0xff | Data Length | Length checksum | Frame Identifier | Packet Data (n bytes) | Packet Data Checksum | Postamble |
        /// </summary>
        /// <param name="dataBuffer"></param>
        /// <param name="dataBufferLength"></param>
        private static void WriteCommand(byte[] dataBuffer, byte dataBufferLength)
        {
            var sendBufferLength = 8 + dataBufferLength;

            var writeBuffer = new byte[sendBufferLength];

            // Calculate checksum LCS + LEN == 0x00
            // Add 1 to dataBufferLengthe to include the TFI byte
            var lengthCheckSum = (byte) (~(dataBufferLength + 1) + 1);

            // Preamble
            writeBuffer[0] = (Constants.Pn532Preamble);

            // Start Code
            writeBuffer[1] = (Constants.Pn532Startcode1);
            writeBuffer[2] = (Constants.Pn532Startcode2);

            // LEN (data length + TFI byte
            writeBuffer[3] = (byte)(dataBufferLength + 1);

            // LCS Length Checksum
            writeBuffer[4] = (lengthCheckSum);

            // TFI Frame Identifier
            writeBuffer[5] = (Constants.HostToPn532);
            
            // DCS Data Checksum (adding TFI)
            var dataCheckSum = Constants.HostToPn532;

            // Insert data bytes
            for (byte i = 0; i < dataBufferLength; i++)
            {
                writeBuffer[6 + i] = dataBuffer[i];

                // adding PD0 + PD1 + ... + PDn
                dataCheckSum += dataBuffer[i];
            }

            // Insert DCS
            // (TFI + PD0 + PD1 + ... + PDn + DCS) == 0x00
            writeBuffer[6 + dataBufferLength] = ((byte)(~dataCheckSum + 1));

            // Postamble
            writeBuffer[6 + dataBufferLength + 1] = (Constants.Pn532Postamble);

            writeBuffer.PrintBuffer();

            try
            {
                I2CBus.GetInstance().Write(WriteConfiguration, writeBuffer, I2CTimeout);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
        }
    }

    public static class Utility
    {
        /// <summary>
        /// Extension method to print byte buffer to debug output
        /// </summary>
        /// <param name="buffer">byte array to print</param>
        public static void PrintBuffer(this byte[] buffer)
        {
            if (buffer == null)
            {
                Debug.Print("No buffer to print");
                return;
            }

            Debug.Print("Sending Buffer:");
            for (var i = 0; i < buffer.Length; i++)
            {
                Debug.Print(buffer[i].ToHex());
            }
            Debug.Print("-- End of buffer --");
            Debug.Print(" ");
        }


        /// <summary>
        /// Extension method to create formatted hex code from byte
        /// </summary>
        /// <param name="value">byte to format</param>
        /// <returns>0x00 - 0xFF formatted string</returns>
        public static string ToHex(this byte value)
        {
            const string hex = "0123456789ABCDEF";

            var formattedHexCode = "0x" + hex[value >> 4 & 0x0F] + hex[value & 0x0F];

            return formattedHexCode;
        }
    }
}
