# SpaceY (Unity ML-Agents demo project)

This project demonstrates how machine learning algorithm can be used to teach
a rocket to lift off from one planet, then fly and land onto another.

You can find a video that shows how neural network drives the rocket
[here](https://youtu.be/NBz9mrxIMJo).

Rocket has six engines that apply different acceleration at different positions
of rocket body.

You can switch simulation to "Brain: Player" mode, by pressing corresponding button in
the top right corner, in which you can control the rocket with the following keys:

* "Q" - upper left side engine;
* "E" - upper right side engine;
* "A" - bottom left side engine;
* "D" - bottom right side engine;
* "S" - main bottom engine;
* "W" - top stop engine.

## Prerequisites

If you want to run trained models in Unity you need to install TensorFlowSharp
plugin as described [here](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Getting-Started-with-Balance-Ball.md#setting-up-tensorflowsharp-support).

## Training

You can find more detail how to train this simulation [here](Assets/SpaceYRocketLanderDemo/Docs/HOW_TO_TRAIN.MD)
