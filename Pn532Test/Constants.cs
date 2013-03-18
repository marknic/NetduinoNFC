namespace Pn532Test
{
    public class Constants
    {
        public const byte Pn532Preamble = 0x00;
        public const byte Pn532Startcode1 = 0x00;
        public const byte Pn532Startcode2 = 0xFF;
        public const byte Pn532Postamble = 0x00;

        public const byte HostToPn532 = 0xD4;
        public const byte Pn532Tohost = 0xD5;

        public const int Pn532I2CReadbit = 0x01;
        public const int Pn532I2CBusy = 0x00;
        public const int Pn532I2CReady = 0x01;
        public const int Pn532I2CReadytimeout = 20;


        public const int Pn532CommandDiagnose = 0x00;
        public const int Pn532CommandGetfirmwareversion = 0x02;
        public const int Pn532CommandGetgeneralstatus = 0x04;
        public const int Pn532CommandReadregister = 0x06;
        public const int Pn532CommandWriteregister = 0x08;
        public const int Pn532CommandReadgpio = 0x0C;
        public const int Pn532CommandWritegpio = 0x0E;
        public const int Pn532CommandSetserialbaudrate = 0x10;
        public const int Pn532CommandSetparameters = 0x12;
        public const int Pn532CommandSamconfiguration = 0x14;
        public const int Pn532CommandPowerdown = 0x16;
        public const int Pn532CommandRfconfiguration = 0x32;
        public const int Pn532CommandRfregulationtest = 0x58;
        public const int Pn532CommandInjumpfordep = 0x56;
        public const int Pn532CommandInjumpforpsl = 0x46;
        public const int Pn532CommandInlistpassivetarget = 0x4A;
        public const int Pn532CommandInatr = 0x50;
        public const int Pn532CommandInpsl = 0x4E;
        public const int Pn532CommandIndataexchange = 0x40;
        public const int Pn532CommandIncommunicatethru = 0x42;
        public const int Pn532CommandIndeselect = 0x44;
        public const int Pn532CommandInrelease = 0x52;
        public const int Pn532CommandInselect = 0x54;
        public const int Pn532CommandInautopoll = 0x60;
        public const int Pn532CommandTginitastarget = 0x8C;
        public const int Pn532CommandTgsetgeneralbytes = 0x92;
        public const int Pn532CommandTggetdata = 0x86;
        public const int Pn532CommandTgsetdata = 0x8E;
        public const int Pn532CommandTgsetmetadata = 0x94;
        public const int Pn532CommandTggetinitiatorcommand = 0x88;
        public const int Pn532CommandTgresponsetoinitiator = 0x90;
        public const int Pn532CommandTggettargetstatus = 0x8A;

        public const int Pn532ResponseIndataexchange = 0x41;
        public const int Pn532ResponseInlistpassivetarget = 0x4B;


        public const int Pn532Wakeup = 0x55;


    }
}
