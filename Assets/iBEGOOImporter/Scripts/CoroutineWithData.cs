/*
 * a utility class for invoking coroutines.
 * yields access to coroutine values
 */


public class CoroutineWithData {
     public UnityEngine.Coroutine coroutine { get; private set; }
     public object result;
     private System.Collections.IEnumerator target;
     public CoroutineWithData(UnityEngine.MonoBehaviour owner, System.Collections.IEnumerator target) {
         this.target = target;
         this.coroutine = owner.StartCoroutine(Run());
     }
 
     private System.Collections.IEnumerator Run() {
         while(target.MoveNext()) {
             result = target.Current;
             yield return result;
         }
     }
 }


/* Use Example
Put a yield at the end of your coroutine with the value you want to return...
 private IEnumerator LoadSomeStuff( ) {
     WWW www = new WWW("http://someurl");
     yield return www;
     if (String.IsNullOrEmpty(www.error) {
         yield return "success";
     } else {
         yield return "fail";
     }
 }
Invoke it and get the return value using the utility class...
 CoroutineWithData cd = new CoroutineWithData(this, LoadSomeStuff( ) );
 yield return cd.coroutine;
 Debug.Log("result is " + cd.result);  //  'success' or 'fail'
*/