[Skip to content](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#content)

[![PixelEuphoria](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/cropped-PxELogoMediumSquare.png)](https://pixeleuphoria.com/blog/)

[PixelEuphoria](https://pixeleuphoria.com/blog/)

# Notes on Docking/Splitter User Interfaces

This article will be going over a style of user interfaces that allow docking windows and splitting them. It's a common style seen in a lot of advanced editor applications. We'll start with some notes on a few UI patterns that existed before and then cover the basic concept of implementing it.

*This article will mention some applications when examples are needed. I am not affiliated with any applications mentioned or shown on this page.*

Table of Contents

* [Dinosaur Methods of Multiple Dialogs](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-dinosaur-methods-of-multiple-dialogs)
* [Docking And Splitting](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-docking-and-splitting)
* [Getting Access To An Implementation](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-getting-access-to-an-implementation)
* [The Demo](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-the-demo)
* [The Data structure](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-the-data-structure)
* [Topology Constraints](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-topology-constraints)
* [Deletion](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-deletion)
* [Tabs](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-tabs)
* [Arbitrary Placement](https://pixeleuphoria.com/blog/index.php/notes-on-docking-splitter-auis/#htoc-arbitrary-placement)

## Dinosaur Methods of Multiple Dialogs

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/LayoutTypes800.png)

Methods of juggling multiple windows and dialogs in an application.
Left) Modeless spam. Middle) MDI. Right) Docked layout.

There are two main reasons to need additional dialogs. Either a utility dialog is opened, or we want to bring up another document without closing the current document.

A common strategy in very early applications with many dialogs was to let them float as modeless windows.
*Modeless dialogs are dialog windows that can be used along with the application. This is different from modal windows, that pause the application and must be closed before the rest of the application is usable. (e.g., a save dialog*)

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/6011f1.jpg)

ILM's in-house software, editing a Star Wars movie. Original image from [LinuxJournal](https://www.linuxjournal.com/article/6011).

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/800px-Cleaning_up_Fourier_screenshot_gimp_1.png)

An old version of GIMP. Original image from [Wikimedia Commons](https://commons.wikimedia.org/wiki/File%3ACleaning_up_Fourier_screenshot_gimp_1.png).

Examples of applications whos strategy for the UI involved many modeless dialog windows.

The issue with this was that it was very cluttered and unorganized. It was also very inefficient to manipulate; if you wanted to move the application from one screen to another, you had to drag every single floating window. Often with these applications, I would spend a ridiculous amount of time shuffling through all the overlapping dialogs looking for a single dialog. And then after I did a little work, I would reshuffle to find the next dialog I needed.

Another old pattern was called [Multiple-Document Interface](https://en.wikipedia.org/wiki/Multiple-document_interface), or MDI for short. This is similar to the previous pattern of having multiple floating dialogs, but these dialogs would float within a single main app window instead of the desktop.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/101005-word-mdi.jpg)

An example of MDI. Two windows inside an application's frame. Original image [Clickomania](https://blog.clickomania.ch/2010/10/05/die-gfatterlibruder-aus-redmond-oder-10-jahre-leiden-an-office/).

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/screenshot.jpg)

An example of an MDI. Original image by Fabian Tang, [Codeproject](https://www.codeproject.com/Articles/12514/Multi-Document-Interface-MDI-tab-page-browsing-wit).

Examples of MDI.

## Docking And Splitting

These days, these clunkier patterns have been mostly relegated to history or legacy software. They have fallen out of fashion for UI patterns that allow docking content and placing them with a layout engine.

These newer UIs have a data structure that defines adjacency of dialogs and regions, which is then used to position and resize dialogs with a layout engine. To make things easy and intuitive for the user, there is a drag and drop interface to dock and rip dialogs from the layout. Often overlapping windows are allowed with a notebook [tab interface](https://en.wikipedia.org/wiki/Tab_%28interface%29) to switch between the windows.

Along with tiled placement, the area in-between windows are draggable sashes that allow resizing the areas of the layout.

Many source code IDEs use this to view multiple documents and utility dialogs at once. For example, Visual Studio.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/image-43.png)

Visual studio allowing a custom layout of various windows.

Many of these UI systems also allow hovering dialog that can be placed on different monitors, making use of screen real estate on multiple screens.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/image-44.png)

An example of a docking system making use of multiple screens.
Visual Studio is taking up two entire screens. One screen is dedicated to source editing, one screen is dedicated to debugging, and one screen is dedicated to viewing the application.

The style is also popular amongst Digital Content Creation (DCC) software. This is because there is often a need for a lot of features, but depending on your current task and workflow preferences, what UI controls are needed, and their optimal placement will vary.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/image-45.png)

Blender has many of these layout features, but has an uncommon interface for it.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/image-46.png)

Unity3D uses a docked tab UI.
Sidenote: Unity allows scripting custom editor windows that directly integrate into the IDE's UI.

This pattern has many similarities to MDI or having many floating modeless dialogs, but integrates features for easily and intuitively managing the views. Also, because documents can hover, it's a superset of the modeless dialogs approach.

## Getting Access To An Implementation

Advanced UI libraries often have this feature. It's either already supported in the OS, or there are 3rd party libraries that implement it.

That being said, we're going to cover a basic implementation of it for educational reasons. Implementing the basics of these systems is mostly proper event handling, rectangle calculations, and data structures.

## The Demo

Here's the interactable demo of the algorithm and source we'll be covering.

The source code for the Unity Project is available on [GitHub](https://github.com/Reavenk/UIDock_Dev).

Fullscreen

* "Add Window" can be pressed to add a new hovering window to experiment with.
* "Cascade" is a convenient feature to undock everything.
* Dragging a window to the center of a blank canvas will make it the layout's root.
* Dragging a window to the edge of a docked window will provide a docking preview. If the mouse is released on the preview, docking will occur.
* Docked windows can have their title bars dragged to rip them out of the docked layout and turned back into a hovering window.
* Dragging a window into the center of another window (you'll need to drop it into the green square that appears) will create a notebook tab system.

Because this demo is in Unity, hovering windows cannot escape the game region.

## The Data structure

> "To understand recursion, your must first understand recursion."

I'm not sure if there's an "official" way it should be done, but I will go over the theory and execution of how I implemented it. This section is mostly going to be a series of small excerpts and illustrations. Note that the diagrams will have a legend to the left of them.

In the sample code, the node in the tree data structure is called a Dock. Here is a snippet of its definition:

```
1

2

3

4

5

6

7

8

9

10

11

12

13

14

15

16

17

18

19

20

21

// Dock implements a layout node.

public class Dock

{

public enum Type

{

Void,       // Unset or error type.

Window,     // The Dock is a window node.

Horizontal, // The Dock is a horizontal container node

Vertical,   // The Dock is a vertical container node.

Tab         // The Dock is a tab

}

public Dock parent;                 // The parent node.

public Type dockType = Type.Void;   // What type of node is the object?

public Window window = null;        // Reference to the window, only relevant Dock is a window node.

public List<Dock> children = null;  // The children nodes - only relevant if Dock is a container node.

public Rect cachedPlace;                // The location of the node in the layout, calculated from the last layout.

public Vector2 minSize = Vector2.zero;  // The size of the node, calculated from the last layout.

}


```

First off, there's a region of space we're managing. In the diagrams, this will be referred to as the r*oot*. This is the area where docked content will reside. And if we dock a single-window into it, it takes up the entire managing region. Actually, if we have any windows docked, their layout will take up the entire region.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/UIDockRoot.png)

Left) An empty layout without only a root node.
Right) A layout with a single window node parented to the root.

After docking a single window, we can add another one and they will be split. This can happen either horizontally or vertically. I'll often refer to splitting in a certain direction (i.e. horizontally or vertically) as the "grain". And a grain node will refer to either a horizontal or vertical container node.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/UIDockSplit.png)

Left) Two windows stacked vertically. Vertical layout requires parenting the window nodes to a vertical container node.
Right) Two windows stacked horizontally. Horizontal layout requires parenting the window nodes to a horizontal container node.

Note how in the illustrations, in order to split, we have to replace the root container with the proper grain container, and then we can insert multiple windows to be split. We could keep adding more windows to be split if we wanted to. There's no limit except for running low on space and having the layout get awkward.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/UIDockSplitMore.png)

Left) Many window nodes aligned vertically.
Right) Many window nodes aligned horizontally.

But the layout system doesn't only allow us to do that, we can also have horizontal splits inside of vertical, vice versa, and do this to arbitrary depths. And through this process, we can imagine these layouts as tree data structures.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/UIDockMultiLevels-2.png)

Mixing and matching container types through many depths.

## Topology Constraints

When dealing with this data structure, there are some constraints that need to be enforced to maintain sanity.

* Grain nodes can't contain their same grain as direct children.
  + A horizontal container node can't contain a direct child that's a horizontal node.
  + A vertical container node can't contain a direct child that's a vertical node.
* There's no theoretical upper limit to the number of children grains can have, only practical limits.
  + i.e., the data structure can get as deep as you want as long as all other constraints are enforced, but in reality, the UI gets clunky and messy at a certain point because of unwieldy density.
* Container (grains) nodes must have more than 1 child or else they get replaced with their child.
  + There's no point in keeping around containers if they're not holding multiple children.

**"Grain nodes can't contain their same grain as direct children."**
So here's a question, what if we allow a vertical split to have a vertical split child in it? Or a horizontal split to have a horizontal split child in it? Well, while this may be possible, in practice, this creates huge complexity when managing the tree data-structure. If we didn't enforce this constraint, some code would be simpler, and some would be more complex – and if we forbid this situation, then this is still true in different ways, but the complexity is more manageable. So if we have a situation where a grained container has a container child with the same grain, we collapse it.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/UIDockSameGrainNo.png)

A grained container cannot have a direct child that's a grained container of the same type. For example, a horizontal container as a direct child of a horizontal container. Instead, a single horizontal container should have all the horizontally aligned windows.

**"Container (grains) nodes must have more than 1 child or else they get replaced with their child."**
Another rule is that we can't have grains with only one child in them. If that's the case, we're better off getting rid of the grain. If we enforce this constraint, this allows us to make assumptions in different parts of the code that greatly simplifies things.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/UIDockNoSingles.png)

A container should not have only 1 child. If a situation arises where this might happen, the container should be removed and the window takes its place.

That's pretty much it! We have a graph where a node can either be

* a window
* a container aligning multiple children nodes vertically
* a container aligning multiple children nodes horizontally

## Deletion

The logic for undocking and removal can be deduced from the rules for how the data structure should be maintained and how to add nodes and windows. The task involves removing windows from the data structure while also following the data structure topology constraints.

The biggest issue is detecting when cascading removals are needed. If a container node with two items has one item removed, it will then be left with one child. Since container nodes can't have one child, this means the container must also be removed. To do this, the container's parent needs to replace the container's reference with the single window that's left.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/CascadeDeletion.png)

Deletion of a window when it leaves a parent container with only 1 child afterwards.

This situation can also be complicated if a cascaded deletion leaves a container with a direct child node containing a container of the same grain. If this happens, the data structure needs to be fixed by removing the same grained child. This is done by replacing it with the inner container's children and disposing of the inner container.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/CascadeDeletion2.png)

A scenario where a window's deletion causes 2 cascading deletions.

After the deletion, sashes need to be managed, the layout needs to be recalculated, and windows need to be repositioned and resized.

## Tabs

Coverage of tabs is going to be omitted because it's somewhat involved – although it uses a lot of the basic concepts for horizontal and vertical grained containers.

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/UIDockNotebook.png)

Adding a tab container node.

The biggest issues are managing the extra assets for the tabs, and a constraint that only window nodes can exist as children inside of tabs.

There's also the additional state-keeping work of tracking the active tab and making sure its contents are visible while turning off the other windows.

## Arbitrary Placement

This is the ability to dock absolutely anywhere. For example, if I had a vertical split with many windows and wanted to dock a window to the very right, alongside all the vertically split windows, how does the user specify that? How does one tell the docking system that they want the window docked to the very right, instead of placing the window to the right of an individual docked window, forming a row inside?

![](./Notes on Docking_Splitter User Interfaces - PixelEuphoria_files/Ambiguity-1.png)

In certain situations there are ambiguities if we also want to support docking next to grained containers.

To allow either docking option with a drag and drop interface, the docking system first needs to know there are multiple options possible and then provide a way to specify which possibility is their intent. To implement this properly, this check also needs to be done recursively because if the tree is complex enough, there may be more than 2 ambiguities.

Demo built with Unity 2019.4.16f1
Authored and tested in Chrome.
– William Leu. Stay strong, code on.

Search

Search

For questions, comments, corrections, outrage, etc.: e-mail.

[PixelEuphoria](https://pixeleuphoria.com/blog/),
[Proudly powered by WordPress.](https://wordpress.org/)