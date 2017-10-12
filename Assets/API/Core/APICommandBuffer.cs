using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

public class APICommand {
	public string methodName;
	public object[] arguments;

	public APICommand(string methodName, object[] arguments) {
		this.methodName = methodName;
		this.arguments = arguments;
	}

	public void Invoke() {
		MethodInfo methodInfo = APICommandBuffer.GetAPIMethod(methodName);

		if (methodInfo == null)
			return;

		methodInfo.Invoke(null, arguments);
	}
}

public class APICommandBuffer {

	private List<APICommand> _Commands;

	public APICommandBuffer() {
		_Commands = new List<APICommand>();
	}

	public bool AddCommand(string methodName, params object[] arguments) {
		if (APICommandBuffer.GetAPIMethod(methodName) == null)
			return false;

		APICommand command = new APICommand(methodName, arguments);

		_Commands.Add(command);

		return true;
	}

	public void InvokeCommands() {
		foreach (APICommand command in _Commands)
			command.Invoke();
	}

	public static MethodInfo GetAPIMethod(string methodName) {
		Type type = typeof(API);

		return type.GetMethod(methodName);
	}
}
