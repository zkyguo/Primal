#pragma once
#include "CommonHeaders.h"

/// <summary>
/// Includ all operation about Id of an object
/// </summary>
namespace primal::id
{
	using id_type = u32; //Assume that we would no more than 65504 object in game, which contains generation:index

	constexpr u32 generation_bits{ 8 }; //Assume that our game Entity would have no more that 255 generations
	constexpr u32 index_bits{ sizeof(id_type) * 8 - generation_bits }; //Configurate index bits base on id_type and generation bits
	constexpr id_type index_mask{ (id_type(1) << index_bits) -1 }; //mask which allow to get the index according to index bit
	constexpr id_type generation_mask{ (id_type(1) << generation_bits) - 1 };

	constexpr id_type id_mask{id_type(-1)};

	using generation_type = std::conditional_t < generation_bits <= 16, std::conditional_t<generation_bits <= 8, u8, u16>, u32 >; 

	static_assert(sizeof(generation_type) * 8 >= generation_bits);//auto warning if generation type is below generations bits
	static_assert(sizeof(id_type) - sizeof(generation_type) > 0);

	/// <summary>
	/// Check if id of object is valid
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	inline bool isValide(id_type id)
	{
		return id != id_mask;
	}

	/// <summary>
	/// Get the index of object
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	inline id_type index(id_type id)
	{
		return id & index_mask;
	}

	/// <summary>
	/// Get the generation of object
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	inline id_type generation(id_type id)
	{
		return (id >> index_bits) & generation_mask;
	}

	/// <summary>
	/// Increment generation id while adding a new object
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	inline id_type newGeneration(id_type id)
	{
		const id_type generation{ id::generation(id) + 1 };
		assert(generation < 255);
		return index(id) | (generation << index_bits);

	}
#if _DEBUG

namespace internal{

	struct id_base
	{
		constexpr explicit id_base(id_type id) : _id{ id } {};
		constexpr operator id_type() const { return _id; }

		private : id_type _id;

	};

}

#define DEFINE_TYPED_ID(name)                                        \
	struct name final : id::internal::id_base                        \
	{                                                                \
		constexpr explicit name(id::id_type id) : id_base{ id } {}   \
		constexpr name() : id_base{ id::id_mask } {}                 \
                                                                     \
	};                                                               \

#else
#define DEFINE_TYPED_ID(name) using name = id::id_type;
#endif

}