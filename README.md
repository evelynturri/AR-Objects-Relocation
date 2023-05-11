<div id="top"></div>

<br />
<div align="center">
  <h1 align="center">AR Objects Relocation</h1>

  <h3 align="center">
    The app aims to relocate objects in an AR environment, created in previous sessions, through the use of ThinkRealityA3 smart glasses and Snapdragon spaces SDK. 
  </h3>

  ---
  <p align="center"> 
  </p>
</div>

<div align="center">
    <a href=#about-the-project-sparkles>About</a>
    •
    <a href=#requirements-exclamation>Requirements</a>
    •
    <a href=#getting-started-rocket>Getting started</a>
    •
    <a href=#usage-fire>Usage</a>
    •
    <a href=#organization-of-the-app-memo>Organization</a>
    •
    <a href=#implementation-computer>Implementation</a>
    •
    <a href=#problems-bangbang>Problems</a>
    •
    <a href=#further-works-triangular_flag_on_post>Further works</a>
    •
    <a href=#contacts>Contacts</a>
    •
    <a href=#acknowledgments>Acknowledgements</a>
</div>

---

## About the project :sparkles:
This app is an AR project in which it is possible to see holograms in an AR environment through the use of Lenovo ThinkRealityA3 smart glasses. The aim of the project is to relocate objects in an indoor space, which have already been saved by the user in previous sessions. The relocation of the objects has been implemented through the use of Snapdragon Spaces SDK, which can save and upload AR anchors in an offline mode without using any online dataset tools.

The main point of the project is the use of AR Anchors, thanks to which we save the desired object in a certain point, and at the next use of the glasses, the app is able to track the AR anchors and so relocate the objects.

:fire: If you want to know more about the app, you can read all the details in the [project's report](Report.pdf) :fire:

## Requirements :exclamation:
<!-- <div> 
    <h1> Requirements 
    <img align="center" alt="Requirements" width="35px" src="https://cdn1.iconfinder.com/data/icons/flat-and-simple-part-1/128/attention-1024.png" /> 
    </h1>
 </div> -->

- Lenovo ThinkRealityA3 Smart Glasses [[link](https://www.lenovo.com/it/it/thinkrealitya3?orgRef=https%5C%253A%5C%252F%5C%252Fwww.google.com%5C%252F)]
- Motorola edge 30 pro or Motorola edge+ phones
- Snapdragon Spaces SDK 0.11 [[link](https://spaces.qualcomm.com/sdk/)]

> :warning: Any device working with Snapdragon Spaces SDK can be used with this project and application.

## Getting started :rocket:
- Install *Unity* version 2020.3.40 (A different version is not guaranteed to work properly!)
- Clone the repository 
- Open the project via *Unity*
- Set up the *Unity* project as described in the [website](https://docs.spaces.qualcomm.com/unity/setup/SetupGuideUnity.html) of the Snapdragon Spaces SDK documentation 
## Usage :fire:
For the users, who only want to try the application is sufficient to install the Release app via `adb` command. 

For the users who want to see the code or the *Unity* project, is sufficient to open the project as described in the [Getting started](#getting-started-rocket) bullet points.

## Organization of the app :memo:
The application is structured in two modalities: the Admin mode and the Guest mode. As soon as the user enters the application, he/she can see a panel where to choose its modality and so continue the experience in the application:
<div align="center">
  <img align="center" alt="GlobalMode" width="300px" src="ImagesREADME.md\GlobalModeLab.jpg"/> 
</div>

### Admin
The admin mode is structured for the admin user, who decides how to augment the real world. The admin can:
- create and save objects
- completely clear the store and delete all the objects
- load and see the objects in the store
<div align="center">
  <img align="center" alt="AdminMode" width="300px" src="ImagesREADME.md\AdminModeLab.jpg"/> 
</div>

### Guest
In the Guest mode, it is only possible to load all the objects created by the admin and see them in the augmented world.
<div align="center">
  <img align="center" alt="GuestMode" width="300px" src="ImagesREADME.md\GuestModeLab.jpg"/> 
</div>

> :warning: Look around very well in the room, in order to make the glasses map the world and find the features to track the local anchors saved by the user. In this way, only once the smart glasses tracked the points, the holograms can be loaded to the user.

## Implementation :computer:
The project has been developed in *Unity* and all the code is written in C#. In the next subsections I will explain some implementation choices, made due to some problems or for better usage of the application.

### Scenes structure
The project is structured in different scenes, in particular, there is a global scene where there are all the common tools to
use over all the application, and then the other scenes are loaded as Additive scenes to the main one, through the command [`LoadSceneMode.Additive`](https://docs.unity3d.com/ScriptReference/SceneManagement.LoadSceneMode.Additive.html).

In the following diagram is explained how the scenes are connected. 
<div align="center">
  <img align="center" alt="ScenesDiagram" width="300px" src="ImagesREADME.md\ScenesDiagram.png"/> 
</div>


In the repository all the scenes can be found in [Assets/Scenes](Assets/Scenes). For each scene there is a folder with the same name and inside there is the specific scene and a subfolder with the scripts needed only fo that specific scene.

### Local spatial anchors
In this project has been used the [Local Spatial Anchors](https://docs.spaces.qualcomm.com/common/features/GeneralFeatures.html#local-spatial-anchors) feature of the Snapdragon Spaces SDK. 
To each anchor many AR objects are connected to them and once the app can relocate an anchor, all the objects are loaded. 

### Holograms organization
If you want to use your **personal** AR objects you can do it by following the points in Section *Hologram's organization* of the [report](Report.pdf). 

## Problems :bangbang:
There are still some problems with the app, which are caused especially by the instability of Snapdragon Spaces SDK. 

If you interface with one of the following problems, I recommend you to read the Section _Problems_ in the [report](Report.pdf), where there is written how to handle them:
- Application crash during usage
- Difficulty in saving the objects

> :warning: Pay attention to close the application in the right way by triggering the button Quit. Do not remove the glasses until you see the phone’s graphic on the glasses. This step is crucial for saving the data in the right way.

## Further works :triangular_flag_on_post:
- [ ] Keep updated the SDK
- [ ] Add hands in Admin mode
- [ ] Additional features: Hit testing, Image Tracking, ...
- [ ] Add AR marker-based feature, such as a QR Code

## Contacts 
<div>
  If you have any doubt or specific requests you can contact me here 
  <a href="https://www.linkedin.com/in/evelynturri/">
      <img align="center" alt="Evelyn's LinkedIn" width="29px" src="https://cdn2.iconfinder.com/data/icons/social-aquiocons/512/Aquicon-Linkedin.png" />
  </a> !
</div>

## Acknowledgments
Thanks to Lorenzo Orlandi (PhD student at the University of Trento) for having supervised me during the project.

And thanks to [Arcoda s.r.l](https://www.arcoda.it/ws/it/index.html) for giving me the opportunity to work with Lenovo ThinkRealityA3 smartglasses. 