using System;
using System.Collections.Generic;

namespace Ducc.AI;

public class DataContext
{
	private readonly Dictionary<Type, Dictionary<string, object>> _allData = new();

	private void EnsureType<T>()
	{
		if ( !_allData.ContainsKey( typeof( T ) ) )
		{
			_allData[typeof( T )] = new Dictionary<string, object>();
		}
	}

	public bool HasKey( string key )
	{
		foreach( var typedData in _allData.Values )
		{
			if ( typedData.ContainsKey( key ) )
				return true;
		}
		return false;
	}

	public void Set<T>( string key, T value )
	{
		EnsureType<T>();
		_allData[typeof(T)][key] = value;
	}

	public bool Remove<T>( string key )
	{
		EnsureType<T>();
		return _allData[typeof(T)].Remove( key );
	}

	public T Get<T>( string key )
	{
		EnsureType<T>();
		return (T)_allData[typeof( T )][key];
	}

	public bool TryGet<T>( string key, out T value )
	{
		EnsureType<T>();
		var success = _allData[typeof( T )].TryGetValue( key, out var objValue );
		value = (T)objValue;
		return success;
	}
}
