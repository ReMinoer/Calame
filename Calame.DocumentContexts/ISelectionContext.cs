﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Calame.DocumentContexts
{
    public interface ISelectionContext
    {
        event EventHandler CanSelectChanged;
        bool CanSelect(object instance);
        Task SelectAsync(object instance);
        Task SelectAsync(IEnumerable instances);
    }

    public interface ISelectionContext<in T> : ISelectionContext
    {
        bool CanSelect(T instance);
        Task SelectAsync(T instance);
        Task SelectAsync(IEnumerable<T> instances);
    }
}