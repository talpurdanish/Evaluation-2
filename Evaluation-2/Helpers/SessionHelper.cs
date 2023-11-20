
using Newtonsoft.Json;

namespace Evaluation.Helpers
{
    public static class SessionHelper
    {

        public static void SetObjectAsJson(this ISession session, string key, SessionData value)
        {

            try
            {
                session.SetString(key, JsonConvert.SerializeObject(value));
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }

        }

        public static void SetInt(this ISession session, string key, int value)
        {

            try
            {
                session.SetInt32(key, value);
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }

        }

        public static int? GetInt(this ISession session, string key)
        {

            try
            {
                return session.GetInt32(key);
            }
            catch (Exception)
            {

                return -1;
            }

        }

        public static bool SessionExists(this ISession session)
        {
            return session != null;
        }


        public static T? GetObjectFromJson<T>(this ISession session, string key)
        {
            try
            {

                var value = session.GetString(key);
                return value == null ? default : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception e)
            {

                throw new AppException(e.Message);
            }
        }

        public static bool Remove(this ISession session, string key)
        {
            try
            {
                session.Remove(key);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
