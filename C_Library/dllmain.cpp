// dllmain.cpp : Definiuje punkt wejścia dla aplikacji DLL.
#include "pch.h"

#include <cmath>
#include <algorithm>

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

extern "C" __declspec(dllexport) void FilterCpp(unsigned char* pixelData, unsigned char* resultData, int width, int height, int stride) {
    uint8_t* input = static_cast<uint8_t*>(pixelData);
    uint8_t* output = static_cast<uint8_t*>(resultData);


    for (int y = 1; y < height - 1; ++y) {
        for (int x = 1; x < width - 1; ++x) {
            int horizontalSum = 0;
            int verticalSum = 0;

            //horizontalSum = -input[(y - 1) * stride + (x - 1)] + input[(y - 1) * stride + (x + 1)]
            //    - input[y * stride + (x - 1)] + input[y * stride + (x + 1)]
            //    - input[(y + 1) * stride + (x - 1)] + input[(y + 1) * stride + (x + 1)];

            verticalSum = -input[(y - 1) * stride + (x - 1)] - input[(y - 1) * stride + x] - input[(y - 1) * stride + (x + 1)]
                + input[(y + 1) * stride + (x - 1)] + input[(y + 1) * stride + x] + input[(y + 1) * stride + (x + 1)];


            verticalSum = std::abs(verticalSum);
            verticalSum = std::clamp(verticalSum, 0, 255);
            output[y * stride + x] = static_cast<uint8_t>(verticalSum);
        }
    }
}