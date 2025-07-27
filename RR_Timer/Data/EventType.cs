using Race_timer.Resources;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Race_timer.Data;

/// <summary>
/// Represents different types of sport, based on index
/// </summary>
public enum EventType
{
    [Display(Name = "Running", ResourceType = typeof(EventTypeResource))]
    Running = 0,

    [Display(Name = "Inline", ResourceType = typeof(EventTypeResource))]
    Inline = 1,

    [Display(Name = "MountainBike", ResourceType = typeof(EventTypeResource))]
    MountainBike = 2,

    [Display(Name = "Skiing", ResourceType = typeof(EventTypeResource))]
    Skiing = 3,

    [Display(Name = "Swimming", ResourceType = typeof(EventTypeResource))]
    Swimming = 4,

    [Display(Name = "Triathlon", ResourceType = typeof(EventTypeResource))]
    Triathlon = 5,

    [Display(Name = "Duathlon", ResourceType = typeof(EventTypeResource))]
    Duathlon = 6,

    [Display(Name = "Gigathlon", ResourceType = typeof(EventTypeResource))]
    Gigathlon = 7,

    [Display(Name = "Athletics", ResourceType = typeof(EventTypeResource))]
    Athletics = 8,

    [Display(Name = "Motorsports", ResourceType = typeof(EventTypeResource))]
    Motorsports = 9,

    [Display(Name = "Other", ResourceType = typeof(EventTypeResource))]
    Other = 10,

    [Display(Name = "Cycling", ResourceType = typeof(EventTypeResource))]
    Cycling = 11,

    [Display(Name = "Aquathlon", ResourceType = typeof(EventTypeResource))]
    Aquathlon = 12,

    [Display(Name = "BMX", ResourceType = typeof(EventTypeResource))]
    BMX = 13,

    [Display(Name = "WaterSkiing", ResourceType = typeof(EventTypeResource))]
    WaterSkiing = 14,

    [Display(Name = "Wakeboard", ResourceType = typeof(EventTypeResource))]
    Wakeboard = 15,

    [Display(Name = "Rowing", ResourceType = typeof(EventTypeResource))]
    Rowing = 16,

    [Display(Name = "Biathlon", ResourceType = typeof(EventTypeResource))]
    Biathlon = 17,

    [Display(Name = "SpeedSkating", ResourceType = typeof(EventTypeResource))]
    SpeedSkating = 18,

    [Display(Name = "Sailing", ResourceType = typeof(EventTypeResource))]
    Sailing = 19,

    [Display(Name = "Cyclocross", ResourceType = typeof(EventTypeResource))]
    Cyclocross = 20,

    [Display(Name = "Walking", ResourceType = typeof(EventTypeResource))]
    Walking = 21,

    [Display(Name = "BikeTour", ResourceType = typeof(EventTypeResource))]
    BikeTour = 22,

    [Display(Name = "WomensRun", ResourceType = typeof(EventTypeResource))]
    WomensRun = 23,

    [Display(Name = "CrossCountrySkiing", ResourceType = typeof(EventTypeResource))]
    CrossCountrySkiing = 24,

    [Display(Name = "SkiMountaineering", ResourceType = typeof(EventTypeResource))]
    SkiMountaineering = 25,

    [Display(Name = "SkyRun", ResourceType = typeof(EventTypeResource))]
    SkyRun = 26,

    [Display(Name = "Equestrian", ResourceType = typeof(EventTypeResource))]
    Equestrian = 27,

    [Display(Name = "Snowboard", ResourceType = typeof(EventTypeResource))]
    Snowboard = 28,

    [Display(Name = "Canicross", ResourceType = typeof(EventTypeResource))]
    Canicross = 29,

    [Display(Name = "TrialRunning", ResourceType = typeof(EventTypeResource))]
    TrialRunning = 30,

    [Display(Name = "Motocross", ResourceType = typeof(EventTypeResource))]
    Motocross = 31,

    [Display(Name = "SledDog", ResourceType = typeof(EventTypeResource))]
    SledDog = 33,

    [Display(Name = "ObstacleCourseRace", ResourceType = typeof(EventTypeResource))]
    ObstacleCourseRace = 34,

    [Display(Name = "StandupPaddleboarding", ResourceType = typeof(EventTypeResource))]
    StandupPaddleboarding = 35,

    [Display(Name = "Kayak", ResourceType = typeof(EventTypeResource))]
    Kayak = 36
}

/// <summary>
/// Enum extension for localization of event type
/// </summary>
public static class EventTypeExtensions
{
    /// <summary>
    /// Method for retrieving localized name of event type based on resource dictionary
    /// </summary>
    /// <param name="eventType">Event type that should be localized</param>
    /// <returns>Localized event type</returns>
    public static string GetDisplayName(this EventType eventType)
    {
        var field = eventType.GetType().GetField(eventType.ToString());
        var attribute = field?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.GetName() ?? eventType.ToString();
    }
}