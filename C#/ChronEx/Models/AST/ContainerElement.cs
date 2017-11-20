using ChronEx.Models.AST;
using System;

/// <summary>
/// Container element is the parent class for all elements that contain other elements
/// they act as a pipeline accepting requests on behalf ot their elemets 
/// </summary>
public abstract class ContainerElement : Element
{
    public Element ContainedElement { get; set; }
}
