namespace ArtificialisExcogitatoris
{
  using System;

  internal class CommandAttribute : Attribute
  {
    public string Name { get; }

    public string Description { get; }

    public CommandAttribute(string name = null, string desc = null)
    {
      this.Name = name;
      this.Description = desc;
    }
  }
}
