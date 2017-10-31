using System;
using System.Collections.Generic;
using System.Text;

namespace IPScanner
{
	public enum ScanStatus
	{
		Initializing,
		Scanning,
		NotFound,
		Complete,
		Partial
	}
}
