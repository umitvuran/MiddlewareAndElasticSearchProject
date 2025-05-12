namespace MiddlewareAndElasticSearchProject.Test
{

    public abstract class AHuman
    {
        public static string Name { get; set; } = "asdsad";
        public static string Surname { get; set; } = "";
        public abstract string Description { get; set; }
        public string Desc2 { get; set; }

        public static string Save()
        {
            return $"Adı Soyadı: {Name} {Surname}";
        }

        public abstract string Delete();
    }

    public class Teacher : AHuman
    {
        string t = Teacher.Name;
        string s = Teacher.Surname;

        public override string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        

        public string Department { get; set; }

        public override string Delete()
        {
            var x = Desc2;
            var y = Description;
            var z = Surname;
            throw new NotImplementedException();
        }
    }

    public class Student : AHuman
    {
        public int Id { get; set; }
        public override string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string Delete()
        {
            throw new NotImplementedException();
        }
    }
}
