using UnityEngine;
using System.Collections;

public class SendMolecules : MonoBehaviour
{
	public GameObject molecule;
	public GameObject destinationOrganelle;
	private Renderer rend;
	public float spawnInterval = 3f;
	private float timer = 0;
	void Start ()
	{
		rend = GetComponent<Renderer>();
	}
	
	void Update ()
	{
		timer += Time.deltaTime;
		if(rend.enabled && timer > spawnInterval && destinationOrganelle.activeSelf)
		{
			GameObject newMolecule = Instantiate(molecule, transform.position, transform.rotation) as GameObject;
			MoleculeBehavior mol = newMolecule.GetComponent<MoleculeBehavior>();
			mol.destination = transform.localPosition;
			newMolecule.transform.parent = destinationOrganelle.transform;
			timer = 0f;
		}
	}
}
