//###################################################################################################
/*
    Copyright (c) since 2013 - Paul Freund 
    
    Permission is hereby granted, free of charge, to any person
    obtaining a copy of this software and associated documentation
    files (the "Software"), to deal in the Software without
    restriction, including without limitation the rights to use,
    copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the
    Software is furnished to do so, subject to the following
    conditions:
    
    The above copyright notice and this permission notice shall be
    included in all copies or substantial portions of the Software.
    
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
    EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
    OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
    NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
    HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
    WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
    FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
    OTHER DEALINGS IN THE SOFTWARE.
*/
//###################################################################################################

#include "pch.h"
#include "Gamepad.h"

#define _USE_MATH_DEFINES
#include <math.h>

using namespace XInputWrapper;
using namespace Platform;

Gamepad::Gamepad()
{
    
}

GamepadData Gamepad::Poll()
{
    GamepadData packetData;
    packetData.ControllerConnected = false;

    // Update connections
    UpdateConnection();

    // Return if none found
    if(!_isControllerConnected) return packetData;

    // Get data and check if controller is still connected
    if(XInputGetState(0, &_currentState) != ERROR_SUCCESS)
    {
        _isControllerConnected = false;
        _lastEnumTime = ::GetTickCount64();
        return packetData;
    }

    packetData.ButtonA = (_currentState.Gamepad.wButtons & XINPUT_GAMEPAD_A) == XINPUT_GAMEPAD_A;
    packetData.ButtonB = (_currentState.Gamepad.wButtons & XINPUT_GAMEPAD_B) == XINPUT_GAMEPAD_B;
    packetData.ButtonX = (_currentState.Gamepad.wButtons & XINPUT_GAMEPAD_X) == XINPUT_GAMEPAD_X;
    packetData.ButtonY = (_currentState.Gamepad.wButtons & XINPUT_GAMEPAD_Y) == XINPUT_GAMEPAD_Y;

    packetData.LeftTrigger = ((float)_currentState.Gamepad.bLeftTrigger) / MAXBYTE;
    packetData.RightTrigger = ((float)_currentState.Gamepad.bRightTrigger) / MAXBYTE;

    // Left stick
    float leftX = (float)_currentState.Gamepad.sThumbLX;
    float leftY = (float)_currentState.Gamepad.sThumbLY;
    float leftAngleRaw = atan2f(leftX, leftY) * (180.0f / (float)M_PI);
    packetData.LeftAngle = (leftAngleRaw > 0.0f ? leftAngleRaw : (360.0f + leftAngleRaw));

    float leftItensityRaw = sqrtf(abs(powf(leftX, 2) + powf(leftY, 2))) / MAXSHORT;
    packetData.LeftIntensity = (leftItensityRaw > 1.0f ? 1.0f : leftItensityRaw);

    // Right stick
    float rightX = (float)_currentState.Gamepad.sThumbRX;
    float rightY = (float)_currentState.Gamepad.sThumbRY;
    float rightAngleRaw = atan2f(rightX, rightY) * (180.0f / (float)M_PI);
    packetData.RightAngle = (rightAngleRaw > 0.0f ? rightAngleRaw : (360.0f + rightAngleRaw));

    float rightItensityRaw = sqrtf(abs(powf(rightX, 2) + powf(rightY, 2))) / MAXSHORT;
    packetData.RightIntensity = (rightItensityRaw > 1.0f ? 1.0f : rightItensityRaw);

    packetData.ControllerConnected = true;
    return packetData;
}

void Gamepad::UpdateConnection()
{
    // If it is connected, no problem
    if(_isControllerConnected) return;

    // Check if we have waited long enough to check for a connection
    uint64 currentTime = ::GetTickCount64();
    if((currentTime - _lastEnumTime) < _ConnectScanTimeout) return;
    _lastEnumTime = currentTime;

    // Check for controller connection by trying to get the capabilties
    if(XInputGetCapabilities(0, XINPUT_FLAG_GAMEPAD, &_currentCapabilities) != ERROR_SUCCESS) return;

    _isControllerConnected = true;
}