using System;
using System.Runtime.Serialization;
using UnityEngine;

public interface IWorldPosition
{
	Vector3 iGetWorldPosition();
}

public interface ISerializebleObject
{
	void iSerialize(SerializationInfo info);

	void iDeserialize(SerializationInfo info);
}

public interface ILoadableResource : ISerializebleObject
{
	void iSetResourcePath(string resourcePath);

	string iGetResourcePath();

	UnityEngine.Object iGetObject();
}