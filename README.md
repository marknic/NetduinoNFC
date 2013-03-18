NetduinoNFC
===========

Code using I2C attempting to talk to an Adafruit PN532 NFC card through a Netduino Plus 2.

I have the Adafruit PN532 NFC card and have been attempting to get the Netduino Plus 2 card to talk to it using I2C since the NFC card comes configured for I2C.  I've tried isolating everything to figure out what is going on and cannot get the two to talk.  I've been able to trigger a reset of the card after installing a jumper on the card to connect pin D3 with reset but that doesn't help the communication.  I've even tried to run the NFC card with jumper wires from pins on the Netduino to the corresponding pins on the NFC card - this was just to make sure that no shorts on other pins were causing issues.


Every write transaction fails (returns 0 bytes written).  No interrupts are ever triggered.  I've made sure that the packet being sent is structured correctly although even a poorly constructed packet should write to the card it just wouldn't make the NFC do anything.  The problem here seems to be writing the bytes.  Reads are failing in a similar way.

