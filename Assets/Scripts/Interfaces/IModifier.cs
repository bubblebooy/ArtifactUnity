using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IModifier
{
    void ModifyCard();

    void Clone(IModifier originalIModifier);
}
