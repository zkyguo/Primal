#pragma once
#include "ComponentCommon.h"

namespace primal
{
#define INIT_INFO(component) namespace component {struct init_info;}
	INIT_INFO(transfrom);
#undef INIT_INFO

	namespace game_entity
	{
		struct entity_infos
		{
			transform::init_info* transform{nullptr};
		};
		entity_id createGameEntity(const entity_infos& info);
		void removeGameEntity(entity_id id);
		bool isAlive(entity_id id);

	}
	
}