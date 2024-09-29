using System;
using TestDockerNet8.Model;

namespace TestDockerNet8.Interfaces;

public interface ISpecializeService
{
    void Specialize(FissionSpecializeRequest request);
}
