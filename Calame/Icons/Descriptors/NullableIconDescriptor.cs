﻿using System;
using System.ComponentModel.Composition;
using Calame.Icons.Base;

namespace Calame.Icons.Descriptors
{
    [Export(typeof(IIconDescriptorModule))]
    [Export(typeof(ITypeIconDescriptorModule))]
    public class NullableIconDescriptor : TypeReTargetingDescriptorModuleBase
    {
        public override bool Handle(Type type) => type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        protected override Type GetTypeTarget(Type type) => type.GenericTypeArguments[0];
    }
}