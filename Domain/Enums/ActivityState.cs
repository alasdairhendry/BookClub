using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Serialization;

namespace Domain.Enums;

public enum ActivityState
{
    Active,
    Completed,
    Canceled
}