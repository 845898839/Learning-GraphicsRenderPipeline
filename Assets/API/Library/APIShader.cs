using UnityEngine;

public abstract class APIShader {

	public abstract VertData Vertex(AppData i);
	public abstract Color Fragment(VertData i);
}
