using System;

namespace DataBaseContext.Diagrams
{
	public interface IStringOutputData
	{
		void OutputData(Action<string> OutputHandler);
	}
}