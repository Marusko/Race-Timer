using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Race_timer.Data;

/// <summary>
/// Represents different types of sport, based on index
/// </summary>
public enum EventType
{
    [Display(Name = "Running")]
    Running = 0,

    [Display(Name = "Inline")]
    Inline = 1,

    [Display(Name = "Mountain Bike")]
    MountainBike = 2,

    [Display(Name = "Skiing")]
    Skiing = 3,

    [Display(Name = "Swimming")]
    Swimming = 4,

    [Display(Name = "Triathlon")]
    Triathlon = 5,

    [Display(Name = "Duathlon")]
    Duathlon = 6,

    [Display(Name = "Gigathlon")]
    Gigathlon = 7,

    [Display(Name = "Athletics")]
    Athletics = 8,

    [Display(Name = "Motorsports")]
    Motorsports = 9,

    [Display(Name = "Other")]
    Other = 10,

    [Display(Name = "Cycling")]
    Cycling = 11,

    [Display(Name = "Aquathlon")]
    Aquathlon = 12,

    [Display(Name = "BMX")]
    BMX = 13,

    [Display(Name = "Water Skiing")]
    WaterSkiing = 14,

    [Display(Name = "Wakeboard")]
    Wakeboard = 15,

    [Display(Name = "Rowing")]
    Rowing = 16,

    [Display(Name = "Biathlon")]
    Biathlon = 17,

    [Display(Name = "Speed Skating")]
    SpeedSkating = 18,

    [Display(Name = "Sailing")]
    Sailing = 19,

    [Display(Name = "Cyclocross")]
    Cyclocross = 20,

    [Display(Name = "Walking")]
    Walking = 21,

    [Display(Name = "Bike Tour")]
    BikeTour = 22,

    [Display(Name = "Women's Run")]
    WomensRun = 23,

    [Display(Name = "Cross Country Skiing")]
    CrossCountrySkiing = 24,

    [Display(Name = "Ski Mountaineering")]
    SkiMountaineering = 25,

    [Display(Name = "Sky Run")]
    SkyRun = 26,

    [Display(Name = "Equestrian")]
    Equestrian = 27,

    [Display(Name = "Snowboard")]
    Snowboard = 28,

    [Display(Name = "Canicross")]
    Canicross = 29,

    [Display(Name = "Trial Running")]
    TrialRunning = 30,

    [Display(Name = "Motocross")]
    Motocross = 31,

    [Display(Name = "Sled Dog")]
    SledDog = 33,

    [Display(Name = "Obstacle Course Race")]
    ObstacleCourseRace = 34,

    [Display(Name = "Stand-up Paddleboarding")]
    StandupPaddleboarding = 35,

    [Display(Name = "Kayak")]
    Kayak = 36
}

public static class EventTypeExtensions
{
    public static string GetDisplayName(this EventType eventType)
    {
        var field = eventType.GetType().GetField(eventType.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.GetName() ?? eventType.ToString();
    }
}