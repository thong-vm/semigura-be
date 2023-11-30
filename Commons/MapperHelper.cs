using System.Diagnostics;
using System.Reflection;

namespace semigura.Commons
{
    public static class MapperHelper
    {
        public static TDest Map<TDest>(this object srcObj) where TDest : new()
        {
            //return (TDest)AutoMapper.Mapper.Map(src, src.GetType(), typeof(TDest));
            var desObj = new TDest();
            var desProperties = typeof(TDest).GetProperties();
            var srcPropsDict = new Dictionary<string, PropertyInfo>();
            foreach (var srcProp in srcObj.GetType().GetProperties()) { srcPropsDict.Add(srcProp.Name, srcProp); }
            foreach (var desProp in desProperties)
            {
                if (desProp.CanWrite && srcPropsDict.ContainsKey(desProp.Name))
                {
                    try
                    {
                        var srcValue = srcPropsDict[desProp.Name].GetValue(srcObj);
                        desProp.SetValue(desObj, srcValue);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine("[Error][Map][{0}] {1}", desProp.Name, ex.Message);
                    }
                }
            }
            return desObj;
        }

        //public static object Map(this object src, object des)
        //{
        //    //return (TDest)AutoMapper.Mapper.Map(src, src.GetType(), typeof(TDest));
        //    var desProperties = des.GetType().GetProperties();
        //    var dict = new Dictionary<string, PropertyInfo>();
        //    foreach (var prop in src.GetType().GetProperties()) { dict.Add(prop.Name, prop); }
        //    foreach (var prop in desProperties)
        //    {
        //        if (dict.ContainsKey(prop.Name))
        //        {
        //            var value = dict[prop.Name].GetValue(src);
        //            prop.SetValue(des, value);
        //        }
        //    }
        //    return des;
        //}

        public static T Map<T>(this object src, T des)
        {
            //return (TDest)AutoMapper.Mapper.Map(src, src.GetType(), typeof(TDest));
            var desProperties = des.GetType().GetProperties();
            var dict = new Dictionary<string, PropertyInfo>();
            foreach (var prop in src.GetType().GetProperties()) { dict.Add(prop.Name, prop); }
            foreach (var prop in desProperties)
            {
                if (dict.ContainsKey(prop.Name) && prop.CanWrite)
                {
                    var value = dict[prop.Name].GetValue(src);
                    prop.SetValue(des, value);
                }
            }
            return des;
        }
    }
}