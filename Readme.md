# C# Metaballs Visualizer (WPF)

This is a small demo project built with C# and WPF that visualizes **metaballs** (blobby objects) using the **Marching Squares** algorithm.
Originally inspired by: https://youtu.be/6oMZb3yP_H8?si=EUatNBghpAksZ4X_

## Core Idea

- The scene consists of animated circles with influence fields (potential) defined by:

  ```
  f(x, y) = sum_i(f_i(x,y))) - ISO
  ```
- Single element approximated version function:
  ```
  f(x, y) = R^2/((x-X)^2 + (y-Y)^2 + R) // originally R^2 is added and ISO = 1.0, but I like more result with single R and ISO = 2.0-3.0
  ```

- The **Marching Squares** algorithm computes an isoline (contour) at a given threshold level to visualize the combined field of the circles.


## Features

- Real-time metaball animation with boundary reflection
- Adjustable `threshold` to control the "liquid" behavior of blobs
- Clean separation between visualization and field computation
- Easily extendable with gradient backgrounds and visual effects
