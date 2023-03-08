#pragma once
#include <stdint.h>
//unsigned  intergers
using u8 = uint8_t;
using u16 = uint16_t;
using u32 = uint32_t;
using u64 = uint64_t;

//signed intergers
using s8 = int8_t;
using s16 = int16_t;
using s32 = int32_t;
using s64 = int64_t;

constexpr u64 u64_invalid_id{0xffff'ffff'ffff'ffffui64}; // represent -1 which is not 255
constexpr u32 u32_invalid_id{ 0xffff'ffffui32 };
constexpr u16 u16_invalid_id{ 0xffffui16 };
constexpr u8 u8_invalid_id{ 0xffui8 };

using f32 = float;