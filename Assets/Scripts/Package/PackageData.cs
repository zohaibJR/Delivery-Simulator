using UnityEngine;

public enum PackageStatus
{
    Waiting,
    Picked,
    Delivered
}


public class PackageData : MonoBehaviour
{
    public string packageID;
    public PackageStatus status = PackageStatus.Waiting;

    [Header("Package Address")]
    public HouseAddress address;  // Now enum dropdown

    void Start()
    {
        // If you want random automatic assignment:
        // address = (HouseAddress)Random.Range(0, System.Enum.GetValues(typeof(HouseAddress)).Length);
    }
}
