
using SingletonProject;

var s1 = Singleton.GetInstance();
var s2 = Singleton.GetInstance();

var areSameInstance = ReferenceEquals(s1, s2);

Console.WriteLine($"Are s1 and s2 the same instance? {areSameInstance}");
