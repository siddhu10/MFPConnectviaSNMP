#pragma once
#define DllExport   __declspec( dllexport ) 

class DllExport Calculator
{
public:
	Calculator();
	int Add(int i, int j);
	~Calculator();
};

