# SimulatedLiquid
### This is a project to simulate fluids on a basic level in unity using large scale particles.
#### This projects concept and implementation are based upon this amazing [Youtube video](https://www.youtube.com/watch?v=rSKMYc1CQHE&t=45s&ab_channel=SebastianLague) by Sebastian Lague, as well as this research paper on [Particle-based Fluid Simulation](http://www.ligum.umontreal.ca/Clavet-2005-PVFS/pvfs.pdf) the video is based upon.

## TODO:
1. ~~Extrapolate collision detection from empty GameObject and add it into script on the particle prefab~~
2. Optimize runtime speed to allow for more particles (partially finished) 
4. ~~Implement predictive position equation for calculating density / pressure~~
5. ~~Implement viscosity~~
6. Add color by velocity function for the particles


## Progress Images:

### Small amount of particles with basic rules governing their movement such as a target density determined by a smoothing radius and a pressure multiplier. No gravity.
![](Screenshot%202023-10-14%20213558.png)

### 500 particles with more rules implemented such as viscosity and predictive positioning for calculating density. Gravity enabled. (Settings on the right)
![frame 1](Screenshot%202023-10-14%20215722.png)
![frame 2](Screenshot%202023-10-14%20215739.png)
![frame 3](Screenshot%202023-10-14%20215752.png)
