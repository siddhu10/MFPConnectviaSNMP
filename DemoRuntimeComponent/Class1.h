#pragma once

namespace DemoRuntimeComponent
{
    public ref class CalculatorWrapper sealed
    {
    public:
		CalculatorWrapper();
		int Add(int i, int j);
    };
}