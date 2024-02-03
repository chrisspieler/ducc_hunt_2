using System.Collections.Generic;

namespace Ducc.AI;

public class DataContext
{
	// float
	public void Set( string key, float value ) => _floatData[key] = value;
	public float GetFloat( string key ) => _floatData[key];
	public bool TryGetFloat( string key, out float value ) => _floatData.TryGetValue( key, out value );
	private Dictionary<string, float> _floatData = new();
	// Vector3
	public void Set( string key, Vector3 value ) => _vector3Data[key] = value;
	public Vector3 GetVector3( string key ) => _vector3Data[key];
	public bool TryGetVector3( string key, out Vector3 value ) => _vector3Data.TryGetValue( key, out value );
	private Dictionary<string, Vector3> _vector3Data = new();
}
