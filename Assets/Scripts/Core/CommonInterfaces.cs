using System;
using System.Runtime.Serialization;
using UnityEngine;

public interface IWorldPosition
{
	Vector3 iGetWorldPosition();
}

public interface IExecutable
{
	void iExecute();
}

public interface ISerializableObject
{
	void iSerialize(SerializationInfo info);

	void iDeserialize(SerializationInfo info);
}

public interface ILoadableResource : ISerializableObject
{
	void iSetResourcePath(string resourcePath);

	string iGetResourcePath();

	UnityEngine.Object iGetObject();
}

public interface IConnectable<T>
{
	bool iConnect(T port);
	void iDisconnect(T port);
}

public interface IActivatable
{
	bool IsEnable{ get; }
	bool IsAction{ get; }
	bool IsActivated{ get; }
	float Scale{ get; }

	float iActivate(float time);
	float iDeactivate(float time);
}

public interface IVisualizationElement
{
	void iVisible();
	void iInvisible();
}