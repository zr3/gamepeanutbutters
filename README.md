# game peanutbutters

goes great with game jams!

## usage

clone this repo and use it as a starting point for a game jam. explore the demo, but delete the demo-specific assets and objects before using this to enter something like Ludum Dare. the assets in `Seedwork` are meant to be starting points that are either deleted or modified, and moved out into more appropriate folders after getting started.

## features

this project provides a framework for unity games, which can be used for game jams. it is a collection of boilerplate that i find myself implementing over and over again. it does not contain anything but the most basic assets, so that you can focus on implementing the game itself.

- basic unity ui menu seedwork
- dialog system
- simple motion components
- screenshotter
- basic juice components
- basic camera components and cinemachine setup
- musicbox system
- screen fading
- overall game state management and seedwork

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

## game framework

this project sets up a basic game framework that implements a very basic but full game loop:

- `Game.scene` is initially loaded.
- `Title.scene` is optionally additively loaded on game start and is controlled by `GameConductor`

the core of `GameConductor` shouldn't need to be edited, so it is saved as a separate file. game-specific stuff can be added to the `Seedwork/GameConductor.cs` file.

the core runs a simple state machine using the `IState` interface to represent states. you can create classes that implement them and "new" them up in the states that want to transition to them, or if you need something more complex (e.g. asset or prefab references), you can implement them as `ScriptableObject`s or `MonoBehaviour`s, add them to the `GameConductor`, and reference them that way. see the seedwork for a couple of examples of how this works.

the point of doing this is to make it relatively easy to keep track of and switch between different phases or modes of the game, as well as providing a coroutine update entry point to make it easy to program sequences that depend on time.

the example has a programmatically sequenced intro state, followed by a main game state, and then a credits state (which transitions back to the intro state afterwards). this system could also be used to implement something more complex, like a looping turn-based game:

`intro` -> `gamestart` -> `playerturn` -> `enemyturn` -> `playerturn` -> `enemyturn` -> `...` -> `gameend` -> `gamestart` -> `...`

it just depends on how you program the states and set the `NextState` property in `OnStateExit`. also be sure to isolate the important data for each state within the state, so that they do not depend on others. the `DataDump` and `PersistentGameState` constructs can be one solution for organizing data that needs to be available to multiple states -- or one state could pass data into the constructor or initialization method of the next.

## camera

a basic cinemachine framework has been set up. it uses a statemachine camera which is hooked up to a `Camera` animator asset. this will allow you to easily set properties on the animator and separately handle all of the virtual cameras in a way that fits your game.

## music box

the `MusicBox` allows you to configure music clips and easily line them up to transition on-beat and loop in non-trivial ways.

enable the gameobject in `UI/GameCanvas (above fader)/Debugging` to visualize the state of this in-game to get a better idea of how this is working. currently, it will not update during transitions, but can help with understanding how the looping works.

### music clips

make note of the BPM and beats per bar when you export your music files. then set up `MusicClip`s for each one, and configure them accordingly:

`Clip Layers`: individual music files of the same length that can layer with each other. this could be something like a drum track, accompaniment, and lead for the same few bars of music. it is possible to separately play them via `MusicBox.Instance.FadeInTrack(int index);` and `MusicBox.Instance.FadeOutTrack(int index);`. by default, only the first track is played when calling `MusicBox.Instance.ChangeMusic(int clipIndex);`.

`BPM`: the beats per minute of the music. this is needed to accurately line up music clips.

`Beats per Bar`: the number of beats per bar. this is (probably) usually going to be 3 or 4.

#### alignment properties

`Beginning`: when playing a clip via `MusicBox.Instance.ChangeMusic(int clipIndex);`, this will be the position in the music where the clip starts playing.

`Endings`: these are the allowed exit points in the piece of music.

when `MusicBox.Instance.ChangeMusic(int clipIndex);` is called, the new clip will be lined up in a way that matches its `Beginning` with the current clip's next point in `Endings`. this lets you set things up to only transition at the end of phrases, and on-beat as well.

#### looping properties

`Intro End`: this marks the beginning of the loop, and will probably be 1/1 most of the time. but it can be after the `Beginning` to implement an intro sequence that isn't included in the loop, or before `Beginning` to start playing the music partway through the clip while still looping the whole thing.

`Vamp End`: this marks the ending of the loop, and when the playback reaches this point it will seek back to `Intro End`. if there are no endings configured between `Intro End` and `Vamp End`, when a transition is started, the music will play through the loop and not move to the next clip until an ending is reached.

#### examples

this clip will loop over bars 1 and 2, then play a 2-bar outro before moving on to the next clip.

```
Menu Music
1...2...3...4...
I       V
B               E
```

this will play and loop over the entire 8 bars, but only transition on each 4-bar phrase.

```
Simple looping music
1...2...3...4...5...6...7...8...
I                               V
B               E               E

```

this is a bad example, because it won't work currently. but eventually (if i ever finish implementing it), this should play the first bar while the previous clip is still playing.

```
Music piece with overlapping intro (e.g. ramp-up sounds)
1...2...3...4...5...6...7...8...9...
    I                               V
    B               E               E

```

this will play a 2-bar intro before looping over an 8-bar piece of music.

```
Music piece with intro on-time (e.g. boss music chime)

1...2...3...4...5...6...7...8...9...10..
        I                               V
B                       E               E
```

## poofer

this is a simple script that will play a particle system (non-looping) and audiosource if they exist on the object, but only while the object is moving.

## juicer

this is a class with some static methods that make it easy to add some juice to the game. there is a camera shake method (hooked up with the cinemachine setup) and a method to create automatically pooled gameobjects (like explosions, sounds, bullets, enemies, anything really).

## gobpool

instantiating and destroying gameobjects via `GobPool` will automatically pool them, so that you don't end up with big GC spikes. just be sure to completely reset their state in `OnEnable`.

## messagecontroller

this system is set up to make it easy to present dialogue to the player in a sequenced way, and a basic ui has been set up to avoid some of the busywork. see the demo for examples.

### messagelogger

this component will keep track of messages marked as "logged" by the messagecontroller, so that you can keep track of a set of messages to show the player. this can be used for things like quest objectives or instructions, or whatever else you think of.

## jam checklist

don't forget to do these things

- 🥜 delete the `Assets/_delete` folder. it contains assets used for the demo game.
- 🥜 delete the `_delete` gameobject. it contains demo more game stuff.
- 🥜 update your company name, product name, icons, cursor
- 🥜 update splash image
- 🥜 bake lighting, if needed
- 🥜 take screenshots (`Pause` key by default)
- 🥜 keep track of the credit for third party resources
- 🥜 have some fun

## credit

- legacy sobel edge detection effect by Unity, ported by [jean-moreno](https://github.com/jean-moreno/EdgeDetect-PostProcessingUnity), and packaged by [popcron](https://github.com/popcron/pp-edge-detection)
- peanutbutters icon derived from one made by [Freepik](https://www.flaticon.com/authors/freepik) from [www.flaticon.com](https://www.flaticon.com)
- nohidea idk pack
- soniss.com gdc bundle

### games that helped lead to this

- z88.inject
- Crankship Courier
- FEED
- Sail for Nothing

## roadmap

- build and export script, setup script
- add more features to `MusicBox`, like actual fading
- add support for the new Input System
- separate static parts from seedwork
