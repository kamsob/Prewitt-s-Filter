// dllmain.cpp : Definiuje punkt wejścia dla aplikacji DLL.
#include "pch.h"

#include <cmath>
#include <algorithm>

BOOL APIENTRY DllMain( HMODULE hModule,
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

// Funkcja w C
// pixelData - wsakźnik na obszar obrazu przetwarzany w tym wątku
// outputData - to samo tylko na obrazie wynikowym
// width - długość wiersza w pikselach
// stride - długość wiersza w bajtach
// height - ilość wierszy przetwarzana w tym wątku (endRow - startRow)
// 
//extern "C" __declspec(dllexport) void MyProc2(unsigned char* pixelData, unsigned char* outputData, int width, int stride, int height) {
//    for (int y = 1; y < height - 1; ++y) {
//        for (int x = 1; x < width - 1; ++x) {
//            for (int c = 0; c < 3; c++) { // for each RGB channel
//                // Calculate gradient in the horizontal direction (Gx)
//                int horizontalSum =
//                    -pixelData[((y - 1) * stride + (x - 1) * 3) + c]
//                    - pixelData[(y * stride + (x - 1) * 3) + c]
//                    - pixelData[((y + 1) * stride + (x - 1) * 3) + c]
//                    + pixelData[((y - 1) * stride + (x + 1) * 3) + c]
//                    + pixelData[(y * stride + (x + 1) * 3) + c]
//                    + pixelData[((y + 1) * stride + (x + 1) * 3) + c];
//
//                // Calculate gradient in the vertical direction (Gy)
//                int verticalSum =
//                    -pixelData[((y - 1) * stride + (x - 1) * 3) + c]
//                    - pixelData[((y - 1) * stride + x * 3) + c]
//                    - pixelData[((y - 1) * stride + (x + 1) * 3) + c]
//                    + pixelData[((y + 1) * stride + (x - 1) * 3) + c]
//                    + pixelData[((y + 1) * stride + x * 3) + c]
//                    + pixelData[((y + 1) * stride + (x + 1) * 3) + c];
//
//                // Calculate gradient magnitude
//                int gradient = (int)std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum);
//
//                // Clamp gradient value to the range 0-255
//                gradient = std::clamp(gradient, 0, 255);
//
//                // Write result to output array
//                outputData[(y * stride + x * 3) + c] = (unsigned char)gradient;
//            }
//        }
//    }
//}

extern "C" __declspec(dllexport) void FiltrCpp(unsigned char* resultArray, unsigned char* byteArray, int width, int height, int start, int end)
{
    for (int y = start; y < end; ++y) {
        for (int x = 1; x < width - 1; ++x) {
            for (int c = 0; c < 3; ++c) {
                int horizontalSum =
                    - byteArray[((y - 1) * width + (x - 1)) * 3 + c]
                    - byteArray[(y * width + (x - 1)) * 3 + c]
                    - byteArray[((y + 1) * width + (x - 1)) * 3 + c]
                    + byteArray[((y - 1) * width + (x + 1)) * 3 + c]
                    + byteArray[(y * width + (x + 1)) * 3 + c]
                    + byteArray[((y + 1) * width + (x + 1)) * 3 + c];

                int verticalSum =
                    - byteArray[((y - 1) * width + (x - 1)) * 3 + c]
                    - byteArray[((y - 1) * width + x) * 3 + c]
                    - byteArray[((y - 1) * width + (x + 1)) * 3 + c]
                    + byteArray[((y + 1) * width + (x - 1)) * 3 + c]
                    + byteArray[((y + 1) * width + x) * 3 + c]
                    + byteArray[((y + 1) * width + (x + 1)) * 3 + c];

                int gradient = static_cast<int>(std::sqrt(horizontalSum * horizontalSum + verticalSum * verticalSum)); 

                resultArray[(y * width + x) * 3 + c] =  static_cast<unsigned char>(gradient);
            }
        }
    }
}

