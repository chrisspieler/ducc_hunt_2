using System;
using System.Collections.Generic;

namespace Ducc.AI;

public class DataContext
{
	public bool HasKey( string key ) => 
			_boolData.ContainsKey( key ) 
		||	_intData.ContainsKey( key ) 
		||	_floatData.ContainsKey( key ) 
		||	_vector3Data.ContainsKey( key ) 
		||	_guidData.ContainsKey( key );

	// bool
	public void Set( string key, bool value ) => _boolData[key] = value;
	public bool RemoveBool( string key ) => _boolData.Remove( key );
	public bool GetBool( string key ) => _boolData[key];
	public bool TryGetBool( string key, out bool value ) => _boolData.TryGetValue( key, out value );
	private readonly Dictionary<string, bool> _boolData = new();
	// int
	public void Set( string key, int value ) => _intData[key] = value;
	public bool RemoveInt( string key ) => _intData.Remove( key );
	public int GetInt( string key ) => _intData[key];
	public bool TryGetInt( string key, out int value ) => _intData.TryGetValue( key, out value );
	private readonly Dictionary<string, int> _intData = new();
	// float
	public void Set( string key, float value ) => _floatData[key] = value;
	public bool RemoveFloat( string key ) => _floatData.Remove( key );
	public float GetFloat( string key ) => _floatData[key];
	public bool TryGetFloat( string key, out float value ) => _floatData.TryGetValue( key, out value );
	private readonly Dictionary<string, float> _floatData = new();
	// Vector3
	public void Set( string key, Vector3 value ) => _vector3Data[key] = value;
	public bool RemoveVector3( string key ) => _vector3Data.Remove( key );
	public Vector3 GetVector3( string key ) => _vector3Data[key];
	public bool TryGetVector3( string key, out Vector3 value ) => _vector3Data.TryGetValue( key, out value );
	private readonly Dictionary<string, Vector3> _vector3Data = new();
	// Guid
	public void Set( string key, Guid value ) => _guidData[key] = value;
	public bool RemoveGuid( string key ) => _guidData.Remove( key );
	public Guid GetGuid( string key ) => _guidData[key];
	public bool TryGetGuid( string key, out Guid value ) => _guidData.TryGetValue( key, out value );
	private readonly Dictionary<string, Guid> _guidData = new();
}
