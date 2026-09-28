using UnityEngine;

public interface IVehicle
{
    void EnterVehile();
    void ExitVehile();


    Transform GetEnterPoint();
    Transform GetExitPoint();
}
