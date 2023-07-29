# Epic Enums
Think the standard C# enums are boring? Yeah, same! Lets improve that.

## What does it solve/improve on?

Common c# enums are a powerfull tool for constant data. They are fast and simple. However they sometimes lack the flexibility that you want in a complex application.

Here is a standard C# enum like we all know: 

```csharp
public enum FruitsEnum
{
    Apple,
    Banana,
    Orange,
    DragonFruit
} 
```
All pretty standard nothing special, however if we want to tie some extra properties to our fruit then things get a little tricky. We could of course add an attribute and make it work:

```csharp
public enum FruitsEnum
{
    [Display(Name = "Apple")]
    Apple,
    [Display(Name = "Banana")]
    Banana,
    [Display(Name = "Orange")]
    Orange,
    [Display(Name = "Dragon Fruit")]
    DragonFruit
} 
```

First of all, this is not very readable and can get messy quickly. Most importantly: To access these values you have to write code using reflection to access these properties: (as per [stackoverflow](https://stackoverflow.com/a/25109103/1345060))

```csharp
public static class Extensions
{
    /// <summary>
    ///     A generic extension method that aids in reflecting 
    ///     and retrieving any attribute that is applied to an `Enum`.
    /// </summary>
    public static TAttribute GetAttribute<TAttribute>(this Enum enumValue) 
            where TAttribute : Attribute
    {
        return enumValue.GetType()
                        .GetMember(enumValue.ToString())
                        .First()
                        .GetCustomAttribute<TAttribute>();
    }
}

public class Foo 
{
    public FruitsEnum Fruit = FruitsEnum.Apple;

    public void DisplayName()
    {
        var fruitDisplayName = Fruit.GetAttribute<DisplayAttribute>();
        Console.WriteLine("Which friot is it?");
        Console.WriteLine (fruitDisplayName.Name);
    } 
}

```

Reflection can be slow and give issues in AOT compiling. Plus it can be kind of tedious.

# A Better enum
(The name epic enums was chosen because it alliterats ever so lovely)

### Installing the package

The epic enums package can be found on [nuget](https://www.nuget.org/packages/EpicEnums).

```dotnet add package EpicEnums```


### Creating an Epic Enum
