#pragma once

#ifdef HSHARP_EXPORTS
#define HSHARP_API __declspec(dllexport)
#else
#define HSHARP_API __declspec(dllimport)
#endif
