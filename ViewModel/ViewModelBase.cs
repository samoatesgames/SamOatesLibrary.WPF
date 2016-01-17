using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SamOatesLibrary.WPF.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="property"></param>
        protected void SetAndNotify<T>(ref T source, T value, Expression<Func<T>> property)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            SetAndNotify(ref source, value, propertyInfo.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetAndNotify<T>(ref T source, T value, String propertyName)
        {
            if (!EqualityComparer<T>.Default.Equals(source, value))
            {
                source = value;
                RaisePropertyChangedNotifaction(propertyName);
            }
        }

        /// <summary>
        /// Raise a property changed event
        /// </summary>
        /// <param name="property">The name of the property to rasie an event for</param>
        protected void RaisePropertyChangedNotifaction(String property)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
