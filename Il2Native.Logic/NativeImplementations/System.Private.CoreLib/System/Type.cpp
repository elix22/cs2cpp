#include "System.Private.CoreLib.h"

namespace CoreLib { namespace System { 
    namespace _ = ::CoreLib::System;
    // Method : System.Type.GetTypeFromHandleUnsafe(System.IntPtr)
    _::RuntimeType* Type::GetTypeFromHandleUnsafe(_::IntPtr handle)
    {
        throw 3221274624U;
    }
    
    // Method : System.Type.GetTypeFromHandle(System.RuntimeTypeHandle)
    _::Type* Type::GetTypeFromHandle(_::RuntimeTypeHandle handle)
    {
        throw 3221274624U;
    }
    
    // Method : System.Type.operator ==(System.Type, System.Type)
	bool Type::op_Equality(_::Type* left, _::Type* right)
	{
		if (left == nullptr && right == nullptr)
		{
			return true;
		}

		if (left == nullptr || right == nullptr)
		{
			return false;
		}

		return object::ReferenceEquals(left->get_UnderlyingSystemType(), right->get_UnderlyingSystemType());
	}
    
    // Method : System.Type.operator !=(System.Type, System.Type)
    bool Type::op_Inequality(_::Type* left, _::Type* right)
    {
		if (left == nullptr && right == nullptr)
		{
			return false;
		}

		if (left == nullptr || right == nullptr)
		{
			return true;
		}

		return !object::ReferenceEquals(left->get_UnderlyingSystemType(), right->get_UnderlyingSystemType());
	}

}}

namespace CoreLib { namespace System { 
    namespace _ = ::CoreLib::System;
}}
