#include <WiFi.h>
#include <esp_wifi.h>
#include <Arduino.h>
#include "ESPNowW.h"
//#define ESPNow_debug
uint8_t Clu_mac[] = {0x36, 0x33, 0x33, 0x33, 0x33, 0x32};
uint8_t Gas_mac[] = {0x36, 0x33, 0x33, 0x33, 0x33, 0x33};
uint8_t Brk_mac[] = {0x36, 0x33, 0x33, 0x33, 0x33, 0x34};
uint16_t ESPNow_send=0;
uint16_t ESPNow_recieve=0;
//bool MAC_get=false;
bool ESPNOW_status =false;
bool ESPNow_update= false;
//https://github.com/nickgammon/I2C_Anything/tree/master


void onRecv(const uint8_t *mac_addr, const uint8_t *data, int data_len) 
{
  memcpy(&ESPNow_recieve, data, sizeof(ESPNow_recieve));
  ESPNow_update=true;
}
void OnSent(const uint8_t *mac_addr, esp_now_send_status_t status)
{

}

 

