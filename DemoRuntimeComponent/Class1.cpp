// Class1.cpp
#include "pch.h"
#include "Class1.h"
#include <Calculator.h>

using namespace DemoRuntimeComponent;
using namespace Platform;

CalculatorWrapper::CalculatorWrapper()
{
}

int CalculatorWrapper::Add(int i, int j)
{
	Calculator* calc = new Calculator();
	return calc->Add(i, j);
}
