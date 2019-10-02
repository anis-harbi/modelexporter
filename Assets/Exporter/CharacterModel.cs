

public class CharacterModel 
{
    string name;
    string description;
    string service;
    Image img;

    public CharacterModel(string name, string description, string service, string url)
    {
        this.name = name;
        this.description = description;
        this.service = service;
        this.img = new Image(url);
    }

}

public class Image
{
    string imageURL;
    public Image(string url) { this.imageURL = url; }
}