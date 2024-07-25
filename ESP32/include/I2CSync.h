#include <Wire.h>
uint8_t I2C_Read;
#define I2C_SDA 32
#define I2C_SCL 33 
#define I2C_DEV_ADDR 0x55
#define I2C_rate 400000
TwoWire I2C_sync(0);
bool I2C_sync_status=false;
bool I2C_data_read=false;
uint8_t I2C_send;
uint32_t iii = 0;
void SynconReceive(int len)
{
    Serial.printf("onReceive[%d]: ", len);
    while (I2C_sync.available()) {
        Serial.write(I2C_sync.read());
    }
    Serial.println();
    /*
    I2C_Read=(uint8_t)I2C_sync.read();
    I2C_data_read=true;
    */
}
void SynconRequest()
{
    I2C_sync.print(iii++);
    I2C_sync.print(" Packets.");
    Serial.println("onRequest");
    Serial.println();
    //I2C_sync.slaveWrite((uint8_t *)I2C_send,sizeof(I2C_send));
}