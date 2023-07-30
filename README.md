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
All pretty standard nothing special, however if we want to tie some extra properties to our fruit then things get a little tricky. We could, of course, add an attribute and make it work:

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
        Console.WriteLine("Which fruit is it?");
        Console.WriteLine (fruitDisplayName.Name);
    } 
}

```

Reflection can be slow and give issues in AOT compiling. Plus it can be kind of tedious.

# Getting Started
(The name epic enums was chosen because it alliterates ever so lovely)

## Installing the package

The epic enums package can be found on [nuget](https://www.nuget.org/packages/EpicEnums).

```dotnet add package EpicEnums```


## Creating your first Epic Enum

Epic Enums works based on source generation, but needs some work from you as a developer.

**Warning:** Epic enums has some guidelines on how to work with them. If you dont follow these guidelines unexpected behaviour might happen.

### The value record


```csharp
using EpicEnums;

namespace SampleApp.NotVegetables;

public partial record Fruit : IEpicEnumValue  // It is very important that this is a partial record
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool LikeAble { get; init; }
}
```

### The enum record

```csharp
using EpicEnums;

namespace SampleApp.NotVegetables;

public partial record Fruits : EpicEnum<Fruit> // It is very important that this is a partial record
{
    public static Fruit Apple { get; } = new() { Name = "Apple", Description = "A red fruit", LikeAble = true };
    public static Fruit Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit", LikeAble = true };
    public static Fruit Orange { get; } = new() { Name = "Orange", Description = "An orange fruit", LikeAble = false }; //In reality i like most fruit. no hate✌️
    public static Fruit DragonFruit { get; } = new() { Name = "Dragon Fruit", Description = "A pink fruit", LikeAble = true };
}
```

## The enum

Now after first build the source generator will do its magic we have access to the FruitsEnum. Together with some extensions added to the partial records we can now use the Fruit record and the enum almost interchangeably.

> **Warning**
> Sometimes Visual Studio will indicate an error, but your application will build. Just restart Visual Studio, and in general it shouldnt be an issue anymore.

```csharp
public class Foo
{
    public FruitsEnum MyFruit { get; set; } = FruitsEnum.Apple;

    public void DisplayName()
    {
        Fruit x = MyFruit;
        Console.WriteLine(x.Name);  //No reflection used here
    }
}
```

## Incremental changes

The source generator runs incrementally, which means that whenever you add a property to the Fruits record it will show up on the enum on save.

```csharp
using EpicEnums;

namespace SampleApp.NotVegetables;

public partial record Fruits : EpicEnum<Fruit>
{
    public static Fruit Apple { get; } = new() { Name = "Apple", Description = "A red fruit", LikeAble = true };
    public static Fruit Banana { get; } = new() { Name = "Banana", Description = "A yellow fruit", LikeAble = true };
    public static Fruit Orange { get; } = new() { Name = "Orange", Description = "An orange fruit", LikeAble = false }; 
    public static Fruit DragonFruit { get; } = new() { Name = "Dragon Fruit", Description = "A pink fruit", LikeAble = true };

    // New Code
    public static Fruit Grape { get; } = new() { Name = "Grape", Description = "A purple fruit", LikeAble = true };
}

```

After pressing save `FruitsEnum.Grape` is instantly accessible in code


# Features

Epic enums was created because I lacked some features from enums. This is the second attempt to making it work since the attempt using reflections wasnt that successful. The features will probably grow whenever new scenarios arise (Or feel free to contribute).

## Enumeration

We can enumerate over the Fruits by u calling `Fruits.Values`.

Lets see it in some blazor code:

```csharp
@foreach (var fruit in Fruits.Values)
{
    <p>@fruit</p>
}

//Output
Fruit { Name = Apple, Description = A red fruit, LikeAble = True }

Fruit { Name = Banana, Description = A yellow fruit, LikeAble = True }

Fruit { Name = Orange, Description = An orange fruit, LikeAble = False }

Fruit { Name = Dragon Fruit, Description = A pink fruit, LikeAble = True }
```


```csharp
@foreach (FruitsEnum fruit in Fruits.Values)
{
    <p>@fruit</p>
}

//Output
Apple

Banana

Orange

DragonFruit
```


## Conversion

For conversions you can best look at the unit test that ill lazily show here:

```csharp
Fruit apple = FruitsEnum.Apple;
apple.Should().Be(Fruits.Apple);

FruitsEnum banana = Fruits.Banana;
banana.Should().Be(FruitsEnum.Banana);

var dragonFruit = (FruitsEnum)Fruits.DragonFruit;
dragonFruit.Should().Be(FruitsEnum.DragonFruit);

var orange = (Fruit)FruitsEnum.Orange;
orange.Should().Be(Fruits.Orange);
```




## Comparison 

```csharp
Fruits.Apple == FruitsEnum.Apple            //True
Fruits.Apple != FruitsEnum.Banana           //True

Fruits.Apple == FruitsEnum.Banana           //False
Fruits.Apple != FruitsEnum.Apple            //False

FruitsEnum.Apple == Fruits.Apple            //True
FruitsEnum.Apple != Fruits.Banana           //True

FruitsEnum.Apple == Fruits.Banana           //False
FruitsEnum.Apple != Fruits.Apple            //False

Fruits.Apple == Fruits.Apple                //True
Fruits.Apple != Fruits.Banana               //True
```

The `Fruit` record is also extended with a private property `FruitsEnum _FruitsValue` that every instance gets filled with the corresponding `FruitsEnum` value. You can choose yourself if you want to expose the property by whatever means you want.

# Generated code

In this section you can see what the source generator will generate with our example. I hope this makes it easier to understand or debug issues. You might find some code not mentioned in the guides or features above. Some of them are intended for features in more complex scenarios, which are not fully released. 

## SampleApp.NotVegetables.FruitsEnum.g.cs

```csharp
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the EpicEnums source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable
namespace SampleApp.NotVegetables;
public enum FruitsEnum
{
    Apple,
    Banana,
    Orange,
    DragonFruit, // For some reason if this comma is not added it wont register the last item, in this case DragonFruit would not be found, weird but okay i guess.
}

```

## SampleApp.NotVegetables.Fruits.Fruit.g.cs

```csharp
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the EpicEnums source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable
using EpicEnums.Exceptions;

namespace SampleApp.NotVegetables;

public partial record Fruit 
{
    internal FruitsEnum? FruitsValue
    {
        init
        {
            _FruitsValue = value;
        }
    }

    private FruitsEnum? _FruitsValue;

    public bool IsFruitsEnum()
    {
        return _FruitsValue is not null;
    }
    public static implicit operator FruitsEnum(Fruit fruit)
    {
        return fruit._FruitsValue ?? throw new UnsupportedValueException();
    }

    public static implicit operator Fruit(FruitsEnum fruit)
    {
        return Fruits.FromEnum(fruit);
    }

    public static bool operator ==(Fruit left, FruitsEnum right)
    {
        return left._FruitsValue == right;
    }
    public static bool operator !=(Fruit left, FruitsEnum right)
    {
        return left._FruitsValue != right;
    }
}

```

## SampleApp.NotVegetables.Fruits.g.cs

```csharp
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the EpicEnums source generator
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

#nullable enable
using System.Collections;
using EpicEnums.Exceptions;

namespace SampleApp.NotVegetables;

public partial record Fruits: IEnumerable<Fruit> 
{
    static Fruits()
    {
        Apple = Apple with { FruitsValue = FruitsEnum.Apple };
Banana = Banana with { FruitsValue = FruitsEnum.Banana };
Orange = Orange with { FruitsValue = FruitsEnum.Orange };
DragonFruit = DragonFruit with { FruitsValue = FruitsEnum.DragonFruit };

    }

    public Fruit this[FruitsEnum fruit]
        => FromEnum(fruit);

    public static Fruit FromEnum(FruitsEnum fruit)
        => fruit switch
            {
                FruitsEnum.Apple => Apple,
FruitsEnum.Banana => Banana,
FruitsEnum.Orange => Orange,
FruitsEnum.DragonFruit => DragonFruit,
_ => throw new UnsupportedValueException($"{ fruit }")

            };


    public static IEnumerable<Fruit> Enumerable()
    {
        yield return Apple;
yield return Banana;
yield return Orange;
yield return DragonFruit;

    }

    public static IEnumerable<Fruit> Values
    {
        get
        {
            return Enumerable();
        }
    }

    public IEnumerator<Fruit> GetEnumerator()
    {
        return Enumerable().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Enumerable().GetEnumerator();
    }
}

```
