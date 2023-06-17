﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Rnd.Models.Nodes;

public abstract class Node : Model
{
    public abstract Prototype Prototype { get; }
    public abstract Guid? ParentId { get; }
    public abstract Node? Parent { get; }
    
    [NotMapped]
    public abstract IReadOnlyList<Node> Children { get; }
}

public enum Prototype : byte
{
    Module,
    Unit,
    Field,
    Method
}