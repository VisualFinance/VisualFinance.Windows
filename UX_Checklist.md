#UX Checklist
 

 
##Input Navigation
 * Can the user naturally and effectively navigate the application without switching input devices? Mouse/Keyboard/Stylus/Touch

##Discoverability
 * Can the user discover functionality in the application without having to have implied knowledge or require a help manual?
 * If the application is sufficiently complex, can the user learn these features quickly and in a manner appropriate for that user (text/video/wiki/forum/chat)? 

##Consistency
 * Colors, Styles, Fonts, Sizes etc.
 * Navigation
 * Language
 * Discovery
 * Device/Platform - Is the application consistent for the given platform. Platform consistency trumps consistency across devices. i.e. your application on iOS should feel like an iOS app, not a copy of the Windows desktop app put on an iOS.
 

##Reactive
## Responsive design
 * Does the view cater for various dimensions (different form factors, resized windows, smaller/larger monitors). 
 Does it make most effective use of the user's space?
 * Does the view react well to changes in dimensions? eg orentation flips, window resizes, screen splits

## Progressive enablement

## Responsive UI - quick feedback on progress, no blocking

## Resilient 
 * Failures are built into the design. Keep the user informed, but limit their need to act
 * Consider silence as an error condition. Ideally build heartbeating argos into services. Make them self discovering e.g. I heartbeat every 1s, on 5s silence consider me failed.
 * Elevate failures to business flows. e.g. Server down, consider using offline cache, or making a voice trade
 * Know about failures before the user does. Proactively tell them we are fixng the issue.
 
 

## Performance
 * Aim to have all action take less than 1second. Long running things, aim to have most done in 3 seconds. After 3 seconds, a user context switch will occur.
 * Measure! Consider your volumes. Volumes of what? Measure what? They are the right questions
   * Types of measurements : Users, latency, trades per user, length of string data, payload sizes, throughput....
   * Data value sizes - min, max and distribution of the size of the value of each type of data e.g. customer names, Orders..
   * Data set sizes - m,m&d of the number of values in a set e.g. number of users, live orders per user, lines per order etc...
   * Precision of data - 4sf is normally fine, but not in finance or Geo
 * 


## Event driven
 * React to Server/System/User events, dont poll. i.e. Push not pull.
 

##Dont hold your user's hostage
 * Provide a cancel button to async tasks
 * Allow the user to exit a workflow or screen
 * Allow the user to close the app when they like
 * Allow the user to uninstall the app leaving nothing behind
 
 
##Localization
 * Static text - Label, titles, images
 * Data - Loic
 * Currency & Dates  
 
##Visual construction
 * Space, line, shape, tone, color, movement, rhythm

##Speed of digestion
 * Can your user easily consume the data they need to make the descions they need to
 * Tables and grids vs Charts
 * Text vs visualizations
 * Speed to achieve - google vs yahoo
 
##Target user demographic
 * Wide distrbution of demographic or a tight well defined set
 * Advanced, intermediate, beginner
 * Teachable
 * Usage - constant/daily/weekly/monthly/rarely
 * Requirement to use the system - free will vs mandated
