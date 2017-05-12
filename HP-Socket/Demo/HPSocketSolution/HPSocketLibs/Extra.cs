using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HPSocketLibs
{
    public class Extra<T>
    {
        ConcurrentDictionary<IntPtr, T> dict = new ConcurrentDictionary<IntPtr, T>();

        /// <summary>
        /// 获取附加信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetExtra(IntPtr key)
        {
            T value;
            if (dict.TryGetValue(key, out value))
            {
                return value;
            }
            return default(T);
        }
        /// <summary>
        /// 设置附加信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetExtra(IntPtr key, T value)
        {
            try
            {
                dict.AddOrUpdate(key, value, (tkey, existingValue) => { return value; });
                return true;
            }
            catch (OverflowException)
            {
                return false;
            }
            catch (ArgumentNullException)
            {
                // 参数为空
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool RemoveExtra(IntPtr key)
        {
            T value;
            return dict.TryRemove(key, out value);
        }
    }
}
