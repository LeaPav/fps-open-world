using UnityEngine;

public interface IVehicle
{
    void EnterVehicle();
    void ExitVehicle();


    Transform GetEnterPoint();
    Transform GetExitPoint();
}
