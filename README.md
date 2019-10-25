# game peanutbutters

goes great with game jams!

## features

this project provides a framework for unity games, which can be used for game jams. it is a collection of boilerplate that i find myself implementing over and over again. it does not contain anything but the most basic assets, so that you can focus on implementing the game itself.

- basic unity ui menus with a theming asset
- dialog system
- simple motion components
- build and export script
- screenshotter
- basic juice components
- basic camera components
- basic sound components
- extra math utilities
- screen fading
- checkpoints

### screen fading

use `ScreenFader` to easily fade the screen in and out for some simple polish. it is a singleton component with a static interface, so it can be called anywhere in your code.

#### simple example

this will fade the screen to black (or whatever image/color you set up on the `FadeCanvas/Fader` image component):

```
ScreenFader.FadeOut();
````

#### less simple example

this will wait 1 second, fade the screen in, wait another 1.2 seconds, then instantiate "SomeRandomPrefab":

```
ScreenFader.FadeInThen(
    () => Instantiate(SomeRandomPrefab),
    1f, 1.2f
);
```

see the C# documentation if you are unfamiliar with lambda/arrow syntax. these methods take `Action`s, so you could also pass them parameterless methods directly:

```
ScreenFader.FadeInThen(Screenshotter.Shoot);
```

could also hook up `UnityEvent`s to this fairly easily. for example by having the `GameConductor` expose some `UnityEvent` parameters and running them after fading when there are game mode state changes.

## usage

clone repo, add folder in unity hub?
run a script to set up name?
add dependency to `Packages/manifest.json`?

## game framework

this project sets up a basic game framework that implements a very basic but full game loop:

- `Game.scene` is initially loaded.
- `Title.scene` is optionally additively loaded on game start and is controlled by `GameConductor`

## music box

```
Menu Music
1...2...3...4...
I       V
B               E

Simple looping music
1...2...3...4...5...6...7...8...
I                               V
B               E               E

Music piece with overlapping intro (e.g. ramp-up sounds)
1...2...3...4...5...6...7...8...9...
    I                               V
    B               E               E

Music piece with intro on-time (e.g. boss music chime)
1...2...3...4...5...6...7...8...9...10..
        I                               V
B                       E               E

```

## jam checklist

don't forget to do these things!

- 🥜 update your company name, product name, icons, cursor
- 🥜 update splash image
- 🥜 bake lighting, if needed
- 🥜 take screenshots
- 🥜 keep track of the credit for third party resources
- 🥜 have some fun

## credit

- legacy sobel edge detection effect by Unity, ported by [jean-moreno](https://github.com/jean-moreno/EdgeDetect-PostProcessingUnity), and packaged by [popcron](https://github.com/popcron/pp-edge-detection)
- peanutbutters icon derived from one made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com)
- nohidea idk pack
- soniss.com gdc bundle

### games that led to this

- z88.inject
- Crankship Courier
- FEED
- Sail for Nothing

## roadmap

- add support for the new Input System
- use Playables for the music box
